using System.Text;
using System.Text.Json;
using Synapse.Application.Interfaces;

namespace Synapse.Worker.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public AiService(IConfiguration config)
    {
        _httpClient = new HttpClient();

        _apiKey = config["DeepSeek:ApiKey"]
            ?? throw new Exception("DeepSeek ApiKey missing");

        _model = config["DeepSeek:Model"]
            ?? throw new Exception("DeepSeek Model missing");
    }

    public async Task<string> SummarizeAsync(string content)
    {
        Console.WriteLine("Sending request to DeepSeek...");

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"Summarize the following content in concise bullet points:\n\n{content}"
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.deepseek.com/chat/completions");

        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseContent);

        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(responseContent);

        var summary = doc
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        Console.WriteLine("Received response from DeepSeek.");

        return summary ?? "No summary generated.";
    }
}