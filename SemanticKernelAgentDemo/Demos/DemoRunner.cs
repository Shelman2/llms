using Microsoft.SemanticKernel;
using SemanticKernelLocalAgentsFull.Security;

namespace SemanticKernelLocalAgentsFull.Demos;

public static class DemoRunner
{
    public static async Task RunEmailAssistantDemo(Kernel kernel)
    {
        Console.WriteLine("📧 Demo 1 - Smart Email Assistant");

        var input = "Can we move our meeting to Friday?";
        PromptFilterMiddleware.Validate(input); // 🛡️ Basic jailbreak prevention

        var result = await kernel.InvokeAsync("email", "Reply", new()
        {
            ["input"] = input
        });

        Console.WriteLine("Response:\n" + result.GetValue<string>() + "\n");
    }

    public static async Task RunTaskManagerDemo(Kernel kernel)
    {
        Console.WriteLine("📋 Demo 2 - Intelligent Task Manager");

        var input = "Prepare launch plan for the new feature.";
        PromptFilterMiddleware.Validate(input);

        var result = await kernel.InvokeAsync("task", "GenerateTasks", new()
        {
            ["input"] = input
        });

        Console.WriteLine("Task List:\n" + result.GetValue<string>() + "\n");
    }

    public static async Task RunSupportAgentDemo(Kernel kernel)
    {
        Console.WriteLine("🎧 Demo 3 - Customer Support Agent");

        var input = "I'm getting a 403 error when trying to log in.";
        PromptFilterMiddleware.Validate(input);

        var result = await kernel.InvokeAsync("support", "AnswerCustomer", new()
        {
            ["input"] = input
        });

        Console.WriteLine("Support Reply:\n" + result.GetValue<string>() + "\n");
    }
}
