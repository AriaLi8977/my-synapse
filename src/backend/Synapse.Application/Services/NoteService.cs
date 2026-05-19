using Synapse.Application.DTOs;
using Synapse.Application.Interfaces;
using Synapse.Domain.Entities;


namespace Synapse.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IMessageBus _messageBus;
    public NoteService(INoteRepository noteRepository, IMessageBus messageBus)
    {
        _noteRepository = noteRepository;
        _messageBus = messageBus;
    }

    public async Task<List<Note>> GetAllAsync(Guid userId)
    {
        return await _noteRepository.GetAllAsync(userId);
    }

    public async Task<Guid> CreateAsync(CreateNoteDto dto, Guid userId)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
        await _noteRepository.AddAsync(note);
        Console.WriteLine($"Publishing content: {note.Content}");
        await _messageBus.PublishAsync(new NoteMessageDto { NoteId = note.Id, Content = note.Content });
        return note.Id;
    }

}