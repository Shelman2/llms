namespace SimpleRAGOllama.Models;

public class ChatMessage
{
    public string Role { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }

    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
        Timestamp = DateTime.UtcNow;
    }
}