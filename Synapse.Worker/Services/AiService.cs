using OpenAI;

public class AiService
{
    private readonly OpenAIClient _client;

    public AiService(IConfiguration config)
    {
        _client = new OpenAIClient(config["OpenAIKey"]);
    }

    public async Task<string> SummarizeAsync(string content)
    {
        var result = await _client.CompletionsEndpoint.CreateCompletionAsync(
            new OpenAI.Completions.CompletionRequest
            {
                Prompt = $"Summarize the following content:\n\n{content}",
                MaxTokens = 100,
                //Temperature = 0.5
            });
        return result.Completions[0].Text;
    }
}