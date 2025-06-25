using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelAgentDemo.Models;
using SemanticKernelLocalAgentsFull.Demos;
using SemanticKernelLocalAgentsFull.Plugins;
using SemanticKernelLocalAgentsFull.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Load configuration from appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var ollamaSettings = config.GetSection("Ollama").Get<OllamaSettings>() ?? new OllamaSettings();

var builder = Kernel.CreateBuilder();

builder.Services.AddHttpClient("ollama-client", client =>
{
    client.BaseAddress = new Uri(ollamaSettings.Endpoint);
    client.Timeout = TimeSpan.FromSeconds(ollamaSettings.TimeoutSeconds);
});

builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("ollama-client");

    return new OllamaChatService(httpClient, ollamaSettings.Model);
});

var kernel = builder.Build();

// Register plugins
kernel.Plugins.AddFromType<EmailPlugin>("email");
kernel.Plugins.AddFromType<TaskPlugin>("task");
kernel.Plugins.AddFromType<SupportPlugin>("support");

Console.WriteLine("🏢 Simple AI Agents with Semantic Kernel Demo");
Console.WriteLine("=============================================\n");

// Run demos
await DemoRunner.RunEmailAssistantDemo(kernel);
await DemoRunner.RunTaskManagerDemo(kernel);
await DemoRunner.RunSupportAgentDemo(kernel);