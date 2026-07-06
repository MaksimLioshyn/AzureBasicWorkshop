# Complete Prompt Template: Setup Modular Skills Workflow System

## Overview
This is a complete, reusable prompt to set up a modular skills-based workflow system in any .NET repository. The system separates work into small, focused, cost-effective tasks.

## How to Use This Prompt
1. Replace placeholders (marked with `{...}`) with your project-specific values.
2. Use the entire prompt in a single message to initialize the system.
3. Adapt the examples and skill descriptions to your tech stack.

---

## THE COMPLETE PROMPT

### Setup Instruction
> I need to set up a modular workflow system in my repository: `{REPO_NAME}`
> 
> **Project Details:**
> - Repository: `{REPO_URL}`
> - Primary project: `{PROJECT_NAME}`
> - Test project: `{TEST_PROJECT_NAME}`
> - Tech stack: `{TECH_STACK}` (e.g., ".NET 10, NUnit, C#")
> - Main language: English
> 
> **Goal:**
> Create a reusable two-phase task workflow with modular skills so that:
> 1. **Phase 1 (Bootstrap)** - creates the base implementation quickly.
> 2. **Phase 2 (Completion)** - finishes the task using smaller focused skills.
> 
> This keeps prompts cheap by reducing context and reusing specific skill templates.
> 
> ---
> 
> ## Required Deliverables
> 
> ### 1. Repository-level Instructions
> Create `AGENTS.md` at the repository root with:
> - Overall workflow requirements for `{PROJECT_NAME}`.
> - Two-phase structure: Create Task → Complete Task Workflow.
> - Format for changelog entries: `Task #N: modified method "MethodName"`.
> - General guidance on keeping changes small and focused.
> 
> ### 2. Copilot Instructions
> Create `.github/copilot-instructions.md` with:
> - Project-specific conventions.
> - Reference to the two-phase workflow.
> - Rules about test creation, documentation in English, and changelog format.
> - Implementation style guidelines.
> 
> ### 3. Project Workflow
> Create `{PROJECT_NAME}/WORKFLOW.md` with:
> - Two-phase structure (bootstrap and completion).
> - Phase 1 requirements (what to bootstrap, what to defer).
> - Phase 2 requirements (which skills to run in order).
> - Test and documentation expectations.
> 
> ### 4. Skill Index
> Create `.github/skills/README.md` with:
> - List of all skills and when to use each.
> - Cost-saving rules (use one skill at a time, narrow scope).
> - Workflow order (the sequence skills are invoked).
> - Brief explanation of the skill hierarchy.
> 
> ### 5. Task-Phase Skills
> Create these files under `.github/skills/`:
> 
> #### `task-intake.md`
> - Purpose: Template for submitting structured tasks.
> - Content: Task #, Title, Description, Acceptance Criteria fields.
> - Usage: Before any work starts.
> 
> #### `create-task.md`
> - Purpose: Bootstrap a task from the intake template.
> - Content:
>   - Input format from task-intake.
>   - Phase 1 workflow steps (create base solution).
>   - What NOT to do: defer XML docs to phase 2.
>   - Cost-saving guidance.
> 
> #### `complete-task-workflow.md`
> - Purpose: Finish a bootstrapped task.
> - Content:
>   - Reference the input requirements (bootstrap must be complete).
>   - Workflow order:
>     1. test-stub-generator
>     2. xml-doc-generator
>     3. changelog-updater
>     4. coverage-expansion
>   - Cost-saving rules (execute one skill at a time).
> 
> ### 6. Focused Skills
> Create these files under `.github/skills/`:
> 
> #### `test-stub-generator.md`
> - Purpose: Create placeholder tests for every public method.
> - Content:
>   - Inspect public methods on production class.
>   - Create one test stub per method.
>   - Use false-resulting assertions for placeholders.
>   - Naming convention: `{ClassName}Tests` for test class.
>   - Example: `OrderProcessor` → `OrderProcessorTests`.
> 
> #### `xml-doc-generator.md`
> - Purpose: Add English XML documentation.
> - Content:
>   - Document the class with English summary.
>   - Document every public method.
>   - Keep comments short and specific.
>   - No private implementation details.
>   - Example style: `<summary>Returns the sum of two values.</summary>`.
> 
> #### `changelog-updater.md`
> - Purpose: Record method-level changes.
> - Content:
>   - Required format: `Task #N: modified method "MethodName"`.
>   - One entry per modified public method.
>   - Keep it consistent and brief.
>   - Create or append to `{PROJECT_NAME}/CHANGELOG.md`.
> 
> #### `coverage-expansion.md`
> - Purpose: Replace placeholders with full test coverage.
> - Content:
>   - Expand existing test stubs only after approval.
>   - Add positive and negative test cases.
>   - Cover edge cases and boundary conditions.
>   - Keep tests readable and deterministic.
>   - Focus areas: success paths, error paths, null handling, boundary values.
> 
> ### 7. Example Implementation (Optional but Recommended)
> Create one example class in `{PROJECT_NAME}` to demonstrate:
> - A simple class with 2-3 public methods.
> - English XML documentation.
> - Corresponding test class with stubs.
> - Initial false-resulting assertions.
> - Entry in `CHANGELOG.md`.
> 
> This shows future developers how to follow the workflow.
> 
> ---
> 
> ## Key Naming Conventions
> - **Test class naming:** `{ClassName}Tests`
> - **Test file naming:** `{ClassName}Tests.cs`
> - **Test namespace:** Same as test project namespace.
> - **Changelog format:** `Task #N: modified method "MethodName"`
> - **Documentation language:** English only.
> 
> ## File Structure (Expected Result)
> ```
> {REPO_NAME}/
> ├── AGENTS.md
> ├── .github/
> │   ├── copilot-instructions.md
> │   └── skills/
> │       ├── README.md
> │       ├── task-intake.md
> │       ├── create-task.md
> │       ├── complete-task-workflow.md
> │       ├── test-stub-generator.md
> │       ├── xml-doc-generator.md
> │       ├── changelog-updater.md
> │       └── coverage-expansion.md
> ├── {PROJECT_NAME}/
> │   ├── WORKFLOW.md
> │   ├── CHANGELOG.md
> │   └── ... (source code)
> └── {TEST_PROJECT_NAME}/
>     └── ... (test code)
> ```
> 
> ## How to Use After Setup
> 
> ### Submit a New Task
> ```
> Task #N: {Task Title}
> Description
> {Description}
> Acceptance Criteria
> - {Criterion 1}
> - {Criterion 2}
> 
> Then run: create-task
> ```
> 
> ### Bootstrap Complete
> After the base solution is reviewed and approved, run:
> ```
> Complete task N using complete-task-workflow
> ```
> 
> This will invoke:
> 1. test-stub-generator
> 2. xml-doc-generator
> 3. changelog-updater
> 4. coverage-expansion (after approval)
> 
> ---
> 
> ## Cost-Saving Notes
> - Keep Phase 1 (bootstrap) minimal — no docs, no full tests.
> - Use one skill at a time in Phase 2 — small context, faster execution.
> - Defer coverage expansion until explicit approval.
> - Reuse skill templates to avoid re-explaining patterns.
> - Keep task scope aligned to acceptance criteria.
> 
> ---
> 
> Please create all the above files now and verify with a build.

---

## Customization Guide

### For Different Project Types

#### If using xUnit instead of NUnit:
In `test-stub-generator.md`, replace:
```
[Test]
public void MyTest()
{
	Assert.That(result, Is.False);
}
```

With:
```
[Fact]
public void MyTest()
{
	Assert.False(result);
}
```

#### If using TypeScript/Node.js:
In skill files, replace:
- `ClassName` → `className` (or per your convention)
- `ClassNameTests` → `className.test.ts`
- `[Test]` → `it("...", () => { })`
- `Assert.That` → `expect(...).toBe(...)`

#### If using Java/Spring:
- Test class: `ClassNameTest` (Maven convention)
- `@Test` instead of `[Test]`
- `assertEquals` instead of `Assert.That`
- `// ` comments instead of `///` XML docs

#### If using Python:
- Test class: `TestClassName` (pytest convention)
- `def test_method_name()` instead of methods
- Docstrings instead of XML docs
- `assert result == expected` instead of assertions

---

## Example Filled-In Prompt (for .NET 10 + NUnit)

```
I need to set up a modular workflow system in my repository: MyDataServiceWorkflow

**Project Details:**
- Repository: https://github.com/myteam/MyDataServiceWorkflow
- Primary project: MyDataServiceWorkflow
- Test project: MyDataServiceWorkflow.Tests
- Tech stack: .NET 10, NUnit, C#
- Main language: English

**Goal:**
Create a reusable two-phase task workflow with modular skills...
[rest of prompt above]
```

---

## After Initial Setup: Verification

Once all files are created, verify with:
1. Build should succeed.
2. Check that all 8 skill files exist.
3. Verify test project compiles.
4. Run any existing tests to confirm no regressions.
5. Create one simple test task to validate the workflow works end-to-end.

---

## Important Notes

- This system is **documentation-driven**, not a runtime framework. Skills are markdown guides for AI agents / developers.
- Each skill is intentionally narrow to keep prompt context small and cost-effective.
- The two-phase split (bootstrap vs. complete) enforces review gates and prevents over-specification.
- All documentation must be in English for consistency.
- Changelog entries create an audit trail of what changed and why.

---

## Next Steps After Setup

1. **Commit** the new files to version control.
2. **Use the workflow** for all new tasks in the project.
3. **Adapt skill content** based on your team's feedback over the first few tasks.
4. **Share** this workflow template with other teams/projects that want the same system.
