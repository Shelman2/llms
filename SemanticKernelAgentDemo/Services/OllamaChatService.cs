using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Net.Http.Json;
using System.Text.Json;

namespace SemanticKernelLocalAgentsFull.Services;

public class OllamaChatService : IChatCompletionService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OllamaChatService(HttpClient httpClient, string model)
    {
        _httpClient = httpClient;
        _model = model;
    }

    public string ModelId => _model;

    public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

    public ChatHistory CreateNewChat(string? instructions = null)
    {
        var chat = new ChatHistory();
        if (!string.IsNullOrWhiteSpace(instructions))
        {
            chat.AddSystemMessage(instructions);
        }
        return chat;
    }

    public async Task<ChatMessageContent> GetChatMessageContentAsync(
        ChatHistory chat,
        PromptExecutionSettings? settings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        var prompt = string.Join("\n", chat.Select(m => $"{m.Role}: {m.Content}"));
        var requestBody = new
        {
            model = _model,
            prompt = prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<OllamaResponse>(json);

        return new ChatMessageContent(AuthorRole.Assistant, result?.response?.Trim() ?? "(no response)");
    }

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chat,
        PromptExecutionSettings? settings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        var single = await GetChatMessageContentAsync(chat, settings, kernel, cancellationToken);
        return new List<ChatMessageContent> { single };
    }

    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chat,
        PromptExecutionSettings? settings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Streaming not implemented.");
    }

    private class OllamaResponse
    {
        public string response { get; set; } = string.Empty;
    }
}