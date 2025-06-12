using SimpleRAGOllama.Models;

namespace SimpleRAGOllama.Settings;
public class AppConfig
{
    public OllamaSettings Ollama { get; set; } = new();
    public List<string> DocumentPaths { get; set; } = new();
    public MemorySettings Memory { get; set; } = new();
}