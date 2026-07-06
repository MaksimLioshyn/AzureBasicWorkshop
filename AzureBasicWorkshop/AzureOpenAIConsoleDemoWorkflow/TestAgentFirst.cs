namespace AzureOpenAIConsoleDemoWorkflow;

/// <summary>
/// Provides basic arithmetic operations for the first agent task.
/// </summary>
public sealed class TestAgentFirst
{
    /// <summary>
    /// Returns the subtraction result of two integers.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns>The value of <paramref name="a"/> minus <paramref name="b"/>.</returns>
    public int Substitute(int a, int b)
    {
        return a - b;
    }

    /// <summary>
    /// Returns the addition result of two integers.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }
}
