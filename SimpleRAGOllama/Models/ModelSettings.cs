namespace SimpleRAGOllama.Models;

public class ModelSettings
{
    public string Name { get; set; } = "";
    public int MaxTokenTotal { get; set; } = 2048;
    public int Seed { get; set; } = 42;
    public int TopK { get; set; } = 7;
}