using Synapse.Application.Interfaces;
using Synapse.Application.DTOs;
using Synapse.Domain.Entities;
using Synapse.Domain.Enums;

namespace Synapse.Application.UseCases;

public class DeleteNoteUseCase
{
    private readonly INoteRepository _noteRep;

    public DeleteNoteUseCase(INoteRepository noteRep)
    {
        _noteRep = noteRep;
    }

    public async Task<bool> ExecuteAsync(Guid id, Guid userId)
    {
        var note = await _noteRep.GetByIdAsync(id);
        if (note == null)
        {
            throw new Exception("Note not found");
        }else if (note.UserId != userId)
        {
            throw new Exception("Access denied");
        }
        await _noteRep.DeleteAsync(id);
        return true;
    }
}