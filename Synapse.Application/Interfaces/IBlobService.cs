
public interface IBlobService
{
    Task<string> UploadAsync(Stream fileStream, string fileName);
}