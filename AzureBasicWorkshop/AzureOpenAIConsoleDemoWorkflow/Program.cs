namespace AzureOpenAIConsoleDemoWorkflow;

public static class Program
{
    private static async Task Main(string[] args)
    {
        // Settings are loaded from appsettings.json for local demo convenience.
        Console.WriteLine("Azure OpenAI settings are loaded from appsettings.json.\n");

        // Practical part: run three Azure OpenAI demos in sequence.
        var workflow = new AzureOpenAIPracticeWorkflow();

        Console.WriteLine("=== Azure OpenAI Practical Demo ===");
        Console.WriteLine();

        var basicResult = await workflow.RunBasicPromptAsync();
        Console.WriteLine("[Demo #1] Basic prompt:");
        Console.WriteLine(basicResult);
        Console.WriteLine();

        var structuredResult = await workflow.RunStructuredOutputAsync(
            "Customer cannot reset password after MFA setup. Error code: AUTH-401-MFA.");
        Console.WriteLine("[Demo #2] Structured output:");
        Console.WriteLine($"Category: {structuredResult.Category}");
        Console.WriteLine($"Severity: {structuredResult.Severity}");
        Console.WriteLine($"Summary: {structuredResult.Summary}");
        Console.WriteLine();

        var businessResult = await workflow.RunBusinessCaseAsync(
        [
            "Ticket #1542: payment callback timeout for EU users.",
            "Ticket #1545: order confirmation email delayed by 20 minutes.",
            "Ticket #1549: duplicate customer profile created during registration."
        ]);

        Console.WriteLine("[Demo #3] Business case (action items):");
        foreach (var item in businessResult.ActionItems)
        {
            Console.WriteLine($"- {item}");
        }
    }
}
