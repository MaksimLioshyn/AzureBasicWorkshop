namespace AzureOpenAIConsoleDemoWorkflow;

/// <summary>
/// Provides a simple example workflow that can be used to demonstrate the repository conventions.
/// </summary>
public sealed class WorkflowGuide
{
    /// <summary>
    /// Returns a short description of the workflow.
    /// </summary>
    /// <returns>A workflow summary string.</returns>
    public string GetSummary()
    {
        return "AzureOpenAIConsoleDemoWorkflow guidance";
    }

    /// <summary>
    /// Indicates whether the workflow is ready for test expansion.
    /// </summary>
    /// <returns><c>true</c> when the workflow can be expanded.</returns>
    public bool IsReadyForCoverageExpansion()
    {
        return true;
    }
}
