namespace SimpleRAGOllama.Models;

public class OllamaSettings
{
    public string Endpoint { get; set; } = "http://localhost:11434/";
    public ModelSettings TextModel { get; set; } = new();
    public ModelSettings EmbeddingModel { get; set; } = new();
}