using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Synapse.Worker.Services;
using Synapse.Domain.Enums;
using Microsoft.Extensions.Options;
using Synapse.Infrastructure.Settings;
using Synapse.Application.DTOs;
namespace Synapse.Worker;

public class NoteProcessWorker : BackgroundService
{
    private readonly ILogger<NoteProcessWorker> _logger;
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;

    public NoteProcessWorker(ILogger<NoteProcessWorker> logger,
                            ServiceBusClient client,
                            IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _processor = client.CreateProcessor("notesqueue", 
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
            }
            ); // to ensure order of processing
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += HandleMessage;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task HandleMessage(ProcessMessageEventArgs args)
    {
        using var scope = _scopeFactory.CreateScope();
        //fix singletons scoped mismatch
        var ai = scope.ServiceProvider.GetRequiredService<IAiService>();
        var noteRepository = scope.ServiceProvider.GetRequiredService<INoteRepository>();
        var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var json = args.Message.Body.ToString();
        _logger.LogInformation($"RAW MESSAGE: {json}");
        var message = System.Text.Json.JsonSerializer.Deserialize<NoteMessageDto>(json);
        if (message == null) return;
        _logger.LogInformation($"Processing note: Id:{message.NoteId}, Content:{message}");

        var noteId = message.NoteId;

        var note = await noteRepository.GetByIdAsync(noteId);
        if (note == null) return;

        note.Status = NoteStatus.Processing;
        await noteRepository.UpdateAsync(note);
        await notifier.NotifyNoteProcessing(note.Id);

        try
        {
            _logger.LogInformation("Starting AI summarization...");
            var summary = await ai.SummarizeAsync(message.Content ?? "");
            _logger.LogInformation("AI summarization completed.");
            _logger.LogInformation($"Summary: {summary}");
            note.Summary = summary;
            note.Status = NoteStatus.Completed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing note {note.Id}");
            note.Status = NoteStatus.Failed;
        }

        await noteRepository.UpdateAsync(note);

        await notifier.NotifyNoteCompleted(note.Id, note.Summary ?? "No summary generated", note.UserId);
        
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Message handler encountered an exception");
        return Task.CompletedTask;
    }
}
