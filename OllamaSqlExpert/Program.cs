using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OllamaSqlExpert;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.Configure<OllamaOptions>(context.Configuration.GetSection("Ollama"));

        // Register ChatClient using OllamaOptions from DI
        services.AddSingleton<IChatClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return new OllamaChatClient(new Uri(options.Endpoint), options.Model);
        });
    });

using var host = builder.Build();
var chatClient = host.Services.GetRequiredService<IChatClient>();

var systemPrompt = @"You are a world-class SQL expert. 
You answer only questions about SQL, databases, and related best practices.
If a question is not about SQL, politely reply: 
'Sorry, I can only answer questions about SQL or databases.'";

// Always keep the system prompt as the first message
var messages = new List<ChatMessage>
{
    new(ChatRole.System, systemPrompt),
};

Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("Ollama .NET Chat Demo (using Microsoft.Extensions.AI)");
Console.WriteLine("Type your SQL or database question and press Enter. Type 'exit' to quit.\n");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("You: ");
    Console.ForegroundColor = ConsoleColor.White;

    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    if (userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    messages.Add(new ChatMessage(ChatRole.User, userInput));

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Assistant: ");
    Console.ForegroundColor = ConsoleColor.White;

    var response = string.Empty;
    await foreach (var update in chatClient.GetStreamingResponseAsync(messages))
    {
        Console.Write(update.Text);
        response += update.Text;
    }

    Console.WriteLine();
    messages.Add(new ChatMessage(ChatRole.Assistant, response));
}
