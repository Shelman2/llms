namespace SemanticKernelAgentDemo.Models;
public class OllamaSettings
{
    public string Model { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 100;
}
