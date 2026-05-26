using Synapse.Application.Interfaces;
using Synapse.Application.DTOs;
using Synapse.Domain.Entities;
using Synapse.Domain.Enums;

namespace Synapse.Application.UseCases;

public class CreateNoteUseCase
{
    private readonly INoteRepository _noteRepo;
    private readonly IMessageBus _mesBus;

    public CreateNoteUseCase(INoteRepository noterepo, IMessageBus mesbus)
    {
        _noteRepo = noterepo;
        _mesBus = mesbus;
    }

    public async Task<Note> ExecuteAsync(string content, Guid userId)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = content,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            Status = NoteStatus.Pending
        };
        if (content == null || content.Length < 10)
        {
            //note.Status = NoteStatus.Error;
            throw new Exception("Content must be at least 10 characters long");
        }

        await _noteRepo.AddAsync(note);
        await _mesBus.PublishAsync(new NoteMessageDto
        {
            NoteId = note.Id,
            Content = note.Content
        });
        return note;
    }
}