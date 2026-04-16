using Microsoft.AspNetCore.Mvc;
using Synapse.Application.DTOs;
using Synapse.Domain.Entities;
using Synapse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IBlobService _blobService;

    public FilesController(IBlobService blob)
    {
        _blobService = blob;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var url = await _blobService.UploadAsync(stream, file.FileName);
        return Ok(url);
    }
}