# Quick Reference Card: Workflow System

## Two-Phase Workflow at a Glance

```
┌─────────────────────────────────────────────────────────────┐
│ PHASE 1: CREATE TASK (Bootstrap)                            │
├─────────────────────────────────────────────────────────────┤
│ 1. Fill task-intake template                                │
│    - Task #                                                 │
│    - Title                                                  │
│    - Description                                            │
│    - Acceptance Criteria                                    │
│                                                             │
│ 2. Run create-task skill                                    │
│    - Creates base production class                          │
│    - Creates test class structure                           │
│    - NO XML documentation                                   │
│    - Keep it minimal and reviewable                         │
│                                                             │
│ 3. Get approval before moving to Phase 2                    │
└─────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────┐
│ PHASE 2: COMPLETE TASK WORKFLOW (Finish)                    │
├─────────────────────────────────────────────────────────────┤
│ Run these skills IN ORDER:                                  │
│                                                             │
│ 1. test-stub-generator                                      │
│    → Creates placeholder tests                             │
│    → One stub per public method                            │
│    → Uses false-resulting assertions                       │
│                                                             │
│ 2. xml-doc-generator                                        │
│    → Adds English XML documentation                        │
│    → Documents class & public methods                      │
│    → Keeps comments concise                                │
│                                                             │
│ 3. changelog-updater                                        │
│    → Records method changes                                │
│    → Format: Task #N: modified method "Name"               │
│    → Updates CHANGELOG.md                                  │
│                                                             │
│ 4. coverage-expansion (after final approval)                │
│    → Replaces placeholder tests                            │
│    → Adds positive & negative cases                        │
│    → Covers edge cases & boundaries                        │
└─────────────────────────────────────────────────────────────┘
```

## Naming Conventions

| Item | Convention | Example |
|------|-----------|---------|
| Production Class | `ClassName` | `OrderProcessor` |
| Test Class | `ClassNameTests` | `OrderProcessorTests` |
| Test File | `ClassNameTests.cs` | `OrderProcessorTests.cs` |
| Test Method | `Method_Scenario_Expected` | `Subtract_WithInputsOneAndTwo_ReturnsMinusOne` |
| Changelog Entry | `Task #N: modified method "Name"` | `Task #5: modified method "Process"` |
| Documentation | English XML comments | `/// <summary>Description</summary>` |

## Commands

### Start a New Task
```
Task #N: {Title}
Description
{Description}

Acceptance Criteria
- {Criterion}

Then: run create-task
```

### Complete a Task
```
Complete task N using complete-task-workflow
```

### Individual Skill (when needed)
```
Use {skill-name}.md for {purpose}

Examples:
- Use test-stub-generator.md to add test stubs
- Use xml-doc-generator.md to add documentation
- Use changelog-updater.md to update changelog
```

## Cost-Saving Rules

✓ **DO THIS:**
- Use one skill at a time
- Keep task scope narrow
- Defer docs to Phase 2
- Defer coverage expansion to Phase 2
- Reuse skill templates

✗ **DON'T DO THIS:**
- Load unrelated project files
- Add XML docs during bootstrap
- Expand tests before approval
- Speculate about extra features
- Mix multiple skills in one prompt

## File Locations

```
{REPO}/
├── AGENTS.md                          ← Repository rules
├── .github/
│   ├── copilot-instructions.md        ← Copilot-specific rules
│   ├── skills/
│   │   ├── README.md                  ← Skill index
│   │   ├── task-intake.md             ← Task template
│   │   ├── create-task.md             ← Bootstrap skill
│   │   ├── complete-task-workflow.md  ← Completion orchestrator
│   │   ├── test-stub-generator.md     ← Test scaffolding
│   │   ├── xml-doc-generator.md       ← Documentation
│   │   ├── changelog-updater.md       ← Change tracking
│   │   └── coverage-expansion.md      ← Full test coverage
│   └── SETUP_WORKFLOW_PROMPT.md       ← This system template
└── {ProjectName}/
	├── WORKFLOW.md                    ← Project-specific workflow
	├── CHANGELOG.md                   ← Change log
	└── ... source code
```

## Quick Decision Matrix

| Need | Use This | Notes |
|------|----------|-------|
| Start new task | task-intake | Fill in the template |
| Create base class | create-task | Bootstrap only, minimal scope |
| Add test stubs | test-stub-generator | One stub per public method |
| Add documentation | xml-doc-generator | English XML comments only |
| Record changes | changelog-updater | Task #N format |
| Expand tests | coverage-expansion | Only after approval |
| Review all skills | README.md (in skills/) | Skill index and cost rules |
| Project rules | WORKFLOW.md | Project-specific requirements |
| Repo rules | AGENTS.md | Repository-wide conventions |

## Changelog Example

```markdown
# Changelog

- Task #001: modified method "Subtract"
- Task #001: modified method "Add"
- Task #002: modified method "Multiply"
- Task #002: modified method "Divide"
```

## Test Stub Example

```csharp
[Test]
public void Subtract_WithInputsOneAndTwo_ReturnsMinusOne()
{
	var result = _calculator.Subtract(1, 2);

	Assert.That(result, Is.False);  // Placeholder: replace with real assertion
}
```

## XML Documentation Example

```csharp
/// <summary>
/// Performs arithmetic operations on two input values.
/// </summary>
public sealed class Calculator
{
	/// <summary>
	/// Returns the subtraction result of two input values.
	/// </summary>
	/// <param name="a">The first input value.</param>
	/// <param name="b">The second input value.</param>
	/// <returns>The result of <paramref name="a"/> minus <paramref name="b"/>.</returns>
	public int Subtract(int a, int b)
	{
		return a - b;
	}
}
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Tests not created | Use test-stub-generator skill explicitly |
| No documentation | Use xml-doc-generator skill explicitly |
| Changelog not updated | Use changelog-updater skill explicitly |
| Too much context | Use one skill at a time, not combined |
| Test coverage incomplete | Use coverage-expansion AFTER approval |
| Class not bootstrapped | Run create-task first |

## One-Sheet Summary

1. **Phase 1:** task-intake → create-task (minimal bootstrap)
2. **Phase 2:** test-stub-generator → xml-doc-generator → changelog-updater → coverage-expansion
3. **Rule:** Use one skill at a time for lowest cost
4. **Format:** English docs, `Task #N: modified method "Name"` changelog
5. **Review:** After Phase 1, before Phase 2 expansion
