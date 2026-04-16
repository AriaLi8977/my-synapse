using Azure.Storage.Blobs;
using Synapse.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Synapse.Infrastructure.Services;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _container;

    public BlobService(IConfiguration config)
    {
        var connectionString = config["BlobStorage"];
        var client = new BlobServiceClient(connectionString);
        _container = client.GetBlobContainerClient("files");
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        var blob = _container.GetBlobClient(fileName);
        await blob.UploadAsync(fileStream);
        return blob.Uri.ToString();
    }
}