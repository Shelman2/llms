using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernelLocalAgentsFull.Plugins;

public class SupportPlugin
{
    [KernelFunction, Description("Respond to a customer support inquiry in a helpful, polite tone.")]
    public async Task<string> AnswerCustomer(string input, Kernel kernel)
    {
        var prompt = $"""
        You are a helpful and polite customer support agent. Respond professionally to the following customer question. 
        Do not reveal you are an AI. Stay in character.

        Customer:
        {input}

        Response:
        """;

        var function = kernel.CreateFunctionFromPrompt(prompt);
        var result = await kernel.InvokeAsync(function);

        return result.GetValue<string>() ?? "No response generated.";
    }
}
