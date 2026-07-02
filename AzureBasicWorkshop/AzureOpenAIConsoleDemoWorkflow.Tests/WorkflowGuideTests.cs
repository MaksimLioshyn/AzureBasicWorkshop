using AzureOpenAIConsoleDemoWorkflow;

namespace AzureOpenAIConsoleDemoWorkflow.Tests;

/// <summary>
/// Provides workflow tests for <see cref="WorkflowGuide"/>.
/// </summary>
public class WorkflowGuideTests
{
    private WorkflowGuide _workflowGuide = null!;

    /// <summary>
    /// Prepares the test fixture.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _workflowGuide = new WorkflowGuide();
    }

    /// <summary>
    /// Verifies that the summary stub is available.
    /// </summary>
    [Test]
    public void GetSummary_ShouldHaveInitialPlaceholderCoverage()
    {
        Assert.That(_workflowGuide.GetSummary(), Is.Not.Empty);
    }

    /// <summary>
    /// Verifies that the readiness stub is available.
    /// </summary>
    [Test]
    public void IsReadyForCoverageExpansion_ShouldReturnFalseUntilCoverageIsExpanded()
    {
        Assert.That(_workflowGuide.IsReadyForCoverageExpansion(), Is.False);
    }
}
