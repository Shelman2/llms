namespace SimpleRAGOllama.Models;

public class MemorySettings
{
    public int ChunkSize { get; set; } = 1024;
    public int ChunkOverlap { get; set; } = 200;
    public int MaxResults { get; set; } = 5;
    public double MinRelevance { get; set; } = 0.3;
    public int MaxChatHistory { get; set; } = 10;
}