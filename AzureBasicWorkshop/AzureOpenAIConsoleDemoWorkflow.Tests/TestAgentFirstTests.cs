using AzureOpenAIConsoleDemoWorkflow;

namespace AzureOpenAIConsoleDemoWorkflow.Tests;

/// <summary>
/// Provides tests for <see cref="TestAgentFirst"/>.
/// </summary>
public class TestAgentFirstTests
{
    private TestAgentFirst _testAgentFirst = null!;

    /// <summary>
    /// Prepares the test fixture.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _testAgentFirst = new TestAgentFirst();
    }

    /// <summary>
    /// Verifies that <see cref="TestAgentFirst.Substitute(int, int)"/> returns -1 for input (1,2).
    /// </summary>
    [Test]
    public void Substitute_WithInputsOneAndTwo_ReturnsMinusOne()
    {
        Assert.That(_testAgentFirst.Substitute(1, 2), Is.EqualTo(-1));
    }

    /// <summary>
    /// Verifies that <see cref="TestAgentFirst.Substitute(int, int)"/> handles negative values.
    /// </summary>
    [Test]
    public void Substitute_WithNegativeValues_ReturnsExpectedDifference()
    {
        Assert.That(_testAgentFirst.Substitute(-5, -3), Is.EqualTo(-2));
    }

    /// <summary>
    /// Verifies that <see cref="TestAgentFirst.Add(int, int)"/> returns 3 for input (1,2).
    /// </summary>
    [Test]
    public void Add_WithInputsOneAndTwo_ReturnsThree()
    {
        Assert.That(_testAgentFirst.Add(1, 2), Is.EqualTo(3));
    }

    /// <summary>
    /// Verifies that <see cref="TestAgentFirst.Add(int, int)"/> handles mixed-sign values.
    /// </summary>
    [Test]
    public void Add_WithMixedSignValues_ReturnsExpectedSum()
    {
        Assert.That(_testAgentFirst.Add(10, -4), Is.EqualTo(6));
    }

    /// <summary>
    /// Verifies acceptance criteria for input (1,2).
    /// </summary>
    [Test]
    public void Methods_ShouldMatchAcceptanceCriteria_ForInputOneAndTwo()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_testAgentFirst.Substitute(1, 2), Is.EqualTo(-1));
            Assert.That(_testAgentFirst.Add(1, 2), Is.EqualTo(3));
        });
    }
}
