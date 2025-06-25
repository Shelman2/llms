using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernelLocalAgentsFull.Plugins;

public class TaskPlugin
{
    [KernelFunction, Description("Generate a checklist of tasks for a given goal.")]
    public async Task<string> GenerateTasks(string input, Kernel kernel)
    {
        var prompt = $"""
        You are a productivity expert. Provide a checklist of actionable steps for the following goal:

        Goal:
        {input}

        Checklist:
        """;

        var function = kernel.CreateFunctionFromPrompt(prompt);
        var result = await kernel.InvokeAsync(function);

        return result.GetValue<string>() ?? "No response generated.";
    }
}
