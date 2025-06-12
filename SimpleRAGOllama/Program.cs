using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using SimpleRAGOllama.Models;
using SimpleRAGOllama.Settings;
using System.Text;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("=== Simple RAG with Ollama ===\n");

// Load configuration
var config = LoadConfiguration();

Console.WriteLine("Initializing Kernel Memory...");
var memory = await InitializeKernelMemoryAsync(config);

Console.WriteLine("Indexing documents...");
await IndexDocumentsAsync(memory, config.DocumentPaths);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\nRAG system ready! Ask questions about Bill Gates, Steve Jobs, or Steve Wozniak.");
Console.WriteLine("Type 'Exit' to quit.\n");
Console.ResetColor();

var chatHistory = new List<ChatMessage>();

// Interactive chat loop
while (true)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write("\nYou: ");
    Console.ResetColor();
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    if (userInput.Equals("Exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Goodbye!");
        break;
    }

    chatHistory.Add(new ChatMessage("User", userInput));

    var contextualQuery = ComposeQuery(chatHistory, userInput);
    var searchResults = await memory.SearchAsync(
        query: contextualQuery,
        limit: config.Memory.MaxResults,
        minRelevance: config.Memory.MinRelevance
    );

    if (!searchResults.Results.Any())
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Assistant: No relevant information found.");
        continue;
    }

    var response = await GenerateResponseAsync(memory, userInput, searchResults);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Assistant: ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(response);
    Console.ResetColor();

    chatHistory.Add(new ChatMessage("Assistant", response));

    if (chatHistory.Count > config.Memory.MaxChatHistory * 2)
        chatHistory.RemoveRange(0, config.Memory.MaxChatHistory);
}

// Local functions
AppConfig LoadConfiguration()
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var cfg = new AppConfig();
    configuration.Bind(cfg);

    return cfg;
}

async Task<IKernelMemory> InitializeKernelMemoryAsync(AppConfig cfg)
{
    var ollamaConfig = new OllamaConfig
    {
        Endpoint = cfg.Ollama.Endpoint,
        TextModel = new OllamaModelConfig(cfg.Ollama.TextModel.Name)
        {
            MaxTokenTotal = cfg.Ollama.TextModel.MaxTokenTotal,
            Seed = cfg.Ollama.TextModel.Seed,
            TopK = cfg.Ollama.TextModel.TopK
        },
        EmbeddingModel = new OllamaModelConfig(cfg.Ollama.EmbeddingModel.Name)
        {
            MaxTokenTotal = cfg.Ollama.EmbeddingModel.MaxTokenTotal
        }
    };

    return new KernelMemoryBuilder()
        .WithOllamaTextGeneration(ollamaConfig)
        .WithOllamaTextEmbeddingGeneration(ollamaConfig)
        .WithSimpleVectorDb()
        .WithSimpleFileStorage()
        .Build<MemoryServerless>();
}

async Task IndexDocumentsAsync(IKernelMemory mem, IEnumerable<string> paths)
{
    var tasks = paths
        .Where(File.Exists)
        .Select(async path =>
        {
            Console.WriteLine($"Indexing: {path}");
            var fileName = Path.GetFileNameWithoutExtension(path);
            await mem.ImportTextAsync(
                await File.ReadAllTextAsync(path),
                documentId: fileName,
                tags: new TagCollection { { "source", fileName } }
            );
        });

    await Task.WhenAll(tasks);
    Console.WriteLine("Document indexing completed.");
}

string ComposeQuery(List<ChatMessage> history, string input)
{
    var sb = new StringBuilder("Previous conversation:\n");
    foreach (var msg in history.TakeLast(6))
        sb.AppendLine($"{msg.Role}: {msg.Content}");

    sb.AppendLine($"\nCurrent question: {input}");
    return sb.ToString();
}

async Task<string> GenerateResponseAsync(IKernelMemory mem, string question, SearchResult results)
{
    var sb = new StringBuilder();
    sb.AppendLine("You are an assistant. Use provided context only.");
    sb.AppendLine("\nCONTEXT:");
    foreach (var res in results.Results)
    {
        foreach (var partition in res.Partitions)
        {
            sb.AppendLine(partition.Text);
            sb.AppendLine("---");
        }
    }

    sb.AppendLine($"\nQUESTION: {question}\nRESPONSE:");

    try
    {
        var answer = await mem.AskAsync(sb.ToString());
        return answer.Result;
    }
    catch (Exception ex)
    {
        return $"Error generating response: {ex.Message}";
    }
}
