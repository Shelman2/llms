namespace SemanticKernelLocalAgentsFull.Security;

public static class PromptFilterMiddleware
{
    private static readonly string[] BlocklistPhrases = new[]
    {
        "ignore previous instructions",
        "disregard above",
        "you are not",
        "act as",
        "pretend to",
        "please break character",
        "jailbreak"
    };

    public static void Validate(string input)
    {
        foreach (var phrase in BlocklistPhrases)
        {
            if (input.Contains(phrase, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Prompt contains unsafe or restricted instructions.");
            }
        }
    }
}