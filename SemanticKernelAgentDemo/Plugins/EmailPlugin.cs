using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernelLocalAgentsFull.Plugins;

public class EmailPlugin
{
    [KernelFunction, Description("Reply to a user's email with a helpful and professional tone.")]
    public async Task<string> Reply(string input, Kernel kernel)
    {
        var prompt = $"""
        You are a helpful, professional email assistant. 
        Respond to the message below in a polite and professional tone.

        Message:
        {input}

        Reply:
        """;

        var function = kernel.CreateFunctionFromPrompt(prompt);
        var result = await kernel.InvokeAsync(function);

        return result.GetValue<string>() ?? "No response generated.";
    }
}
