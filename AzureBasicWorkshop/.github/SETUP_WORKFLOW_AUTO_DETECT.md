# Auto-Detect Setup Workflow Prompt for VS 2026

## How to Use

1. Open this file in VS 2026
2. Copy the entire prompt below (starting after "---")
3. Paste into Copilot/Claude
4. The system will auto-detect your project structure
5. All skills, workflows, and instructions are created automatically

---

## THE AUTO-DETECT PROMPT

> I'm running this prompt from Visual Studio 2026 within my current workspace.
> 
> **Auto-Detect My Project:**
> Please analyze my workspace and automatically detect:
> - The primary .NET project (usually the non-test project in the current directory)
> - The test project (usually named `{PrimaryProject}.Tests`)
> - The .NET version from the .csproj file (check TargetFramework property)
> - The test framework in use (NUnit, xUnit, MSTest - from .csproj PackageReference)
> - The repository URL (from .git/config origin remote)
> 
> **My Repository Location:**
> `{INSERT_YOUR_WORKSPACE_ROOT_PATH_HERE}`
> 
> Example: `D:\Study\AzureBasicWorkshop\AzureBasicWorkshop`
> 
> ---
> 
> ## What to Create
> 
> Based on the auto-detected values, create the complete modular workflow system:
> 
> ### 1. Repository-level Instructions
> Create `AGENTS.md` at the repository root with:
> - Workflow requirements for the detected primary project
> - Two-phase structure: Create Task → Complete Task Workflow
> - Format for changelog entries: `Task #N: modified method "MethodName"`
> - General guidance on keeping changes small and focused
> 
> ### 2. Copilot Instructions
> Create `.github/copilot-instructions.md` with:
> - Project-specific conventions based on detected tech stack
> - Reference to the two-phase workflow
> - Rules about test creation, documentation in English, changelog format
> - Implementation style guidelines
> 
> ### 3. Project Workflow
> Create `{DetectedProjectName}/WORKFLOW.md` with:
> - Two-phase structure (bootstrap and completion)
> - Phase 1 requirements (what to bootstrap, what to defer)
> - Phase 2 requirements (which skills to run in order)
> - Test and documentation expectations
> 
> ### 4. Skill Index
> Create `.github/skills/README.md` with:
> - List of all skills and when to use each
> - Cost-saving rules (use one skill at a time, narrow scope)
> - Workflow order (the sequence skills are invoked)
> - Brief explanation of the skill hierarchy
> 
> ### 5. Task-Phase Skills
> Create these files under `.github/skills/`:
> 
> #### `task-intake.md`
> - Purpose: Template for submitting structured tasks
> - Content: Task #, Title, Description, Acceptance Criteria fields
> - Usage: Before any work starts
> 
> #### `create-task.md`
> - Purpose: Bootstrap a task from the intake template
> - Content:
>   - Input format from task-intake
>   - Phase 1 workflow steps (create base solution)
>   - What NOT to do: defer XML docs to phase 2
>   - Cost-saving guidance
> 
> #### `complete-task-workflow.md`
> - Purpose: Finish a bootstrapped task
> - Content:
>   - Reference the input requirements (bootstrap must be complete)
>   - Workflow order:
>     1. test-stub-generator
>     2. xml-doc-generator
>     3. changelog-updater
>     4. coverage-expansion
>   - Cost-saving rules (execute one skill at a time)
> 
> ### 6. Focused Skills
> Create these files under `.github/skills/` using the detected test framework:
> 
> #### `test-stub-generator.md`
> - Purpose: Create placeholder tests for every public method
> - Content:
>   - Inspect public methods on production class
>   - Create one test stub per method
>   - Use appropriate test framework (detected: NUnit, xUnit, MSTest, etc.)
>   - Placeholder assertions that will be replaced later
>   - Naming convention: `{ClassName}Tests` for test class
>   - Example: `OrderProcessor` → `OrderProcessorTests`
>   - Include framework-specific syntax (e.g., [Test] for NUnit, [Fact] for xUnit)
> 
> #### `xml-doc-generator.md`
> - Purpose: Add English XML documentation
> - Content:
>   - Document the class with English summary
>   - Document every public method
>   - Keep comments short and specific
>   - No private implementation details
>   - Example style: `<summary>Returns the sum of two values.</summary>`
> 
> #### `changelog-updater.md`
> - Purpose: Record method-level changes
> - Content:
>   - Required format: `Task #N: modified method "MethodName"`
>   - One entry per modified public method
>   - Keep it consistent and brief
>   - Create or append to `{DetectedProjectName}/CHANGELOG.md`
> 
> #### `coverage-expansion.md`
> - Purpose: Replace placeholders with full test coverage
> - Content:
>   - Expand existing test stubs only after approval
>   - Add positive and negative test cases
>   - Cover edge cases and boundary conditions
>   - Keep tests readable and deterministic
>   - Focus areas: success paths, error paths, null handling, boundary values
>   - Use detected test framework syntax
> 
> ### 7. Project Changelog
> Create `{DetectedProjectName}/CHANGELOG.md` with:
> - Header: `# Changelog`
> - Empty or with example entries showing the format
> 
> ### 8. Example Implementation (Optional but Recommended)
> If the primary project is currently mostly empty or simple:
> - Create one example class in the detected project
> - Include 2-3 public methods demonstrating the pattern
> - Add English XML documentation
> - Create corresponding test class with stubs
> - Initial false-resulting assertions
> - Entry in `CHANGELOG.md`
> 
> This shows future developers how to follow the workflow
> 
> ---
> 
> ## Auto-Detection Hints
> 
> If you cannot auto-detect from the workspace directly, here are the common patterns:
> 
> **To find the primary project:**
> - Look for the non-test project in the workspace
> - Typically named `{SomeName}` (e.g., `AzureOpenAIConsoleDemoWorkflow`)
> - Usually has `Program.cs` or main entry point
> 
> **To find the test project:**
> - Usually named `{PrimaryProject}.Tests`
> - Contains test framework references (NUnit, xUnit, MSTest)
> - Has NUnit `[Test]` or xUnit `[Fact]` attributes
> 
> **To detect test framework:**
> - Check the `.Tests.csproj` for PackageReference entries
> - Look for NUnit, xUnit, or MSTest packages
> - If not found, use NUnit as default
> 
> **To detect .NET version:**
> - Open the `.csproj` file
> - Find `<TargetFramework>` property
> - Example: `<TargetFramework>net10.0</TargetFramework>`
> 
> **To detect repository URL:**
> - Check `.git/config` for `origin` remote
> - Or use `git remote -v` in terminal
> 
> ---
> 
> ## File Structure (Expected Result)
> 
> ```
> {WorkspaceRoot}/
> ├── AGENTS.md
> ├── .github/
> │   ├── copilot-instructions.md
> │   ├── skills/
> │   │   ├── README.md
> │   │   ├── task-intake.md
> │   │   ├── create-task.md
> │   │   ├── complete-task-workflow.md
> │   │   ├── test-stub-generator.md
> │   │   ├── xml-doc-generator.md
> │   │   ├── changelog-updater.md
> │   │   └── coverage-expansion.md
> │   ├── QUICK_REFERENCE.md
> │   └── SETUP_CHECKLIST.md
> ├── {DetectedProjectName}/
> │   ├── WORKFLOW.md
> │   ├── CHANGELOG.md
> │   └── ... (source code)
> └── {DetectedTestProjectName}/
>     └── ... (test code)
> ```
> 
> ---
> 
> ## After Creation
> 
> 1. Run `dotnet build` to verify everything compiles
> 2. Confirm all 8 skill files are created under `.github/skills/`
> 3. Check that `WORKFLOW.md` is created in the primary project folder
> 4. Verify `CHANGELOG.md` exists in the primary project folder
> 5. If example class was created, run tests to ensure they pass
> 
> ---
> 
> ## Important Notes
> 
> - This system is **documentation-driven**, not a runtime framework
> - All documentation must be in English for consistency
> - Skills are markdown guides for AI agents / developers
> - The two-phase split (bootstrap vs. complete) enforces review gates
> - Keep task scope narrow to minimize costs
> - Adjust skill descriptions based on your team's actual workflow

---

## Usage Instructions

### Step 1: Find Your Workspace Root
Open Terminal in VS 2026 and run:
```powershell
pwd  # or Get-Location (PowerShell)
```
Copy the path.

### Step 2: Replace Placeholder
In this file, find the line:
```
**My Repository Location:**
`{INSERT_YOUR_WORKSPACE_ROOT_PATH_HERE}`
```

Replace `{INSERT_YOUR_WORKSPACE_ROOT_PATH_HERE}` with your path, e.g.:
```
**My Repository Location:**
`D:\Study\AzureBasicWorkshop\AzureBasicWorkshop`
```

### Step 3: Copy & Send to Copilot
1. Select all text from "I'm running this prompt..." through the end
2. Copy to clipboard
3. Open Copilot/Claude
4. Paste the prompt
5. Send

### Step 4: Verify
After Copilot creates the files:
- [ ] Build succeeds (`dotnet build`)
- [ ] All 8 skill files exist under `.github/skills/`
- [ ] `WORKFLOW.md` exists in primary project
- [ ] `AGENTS.md` exists at repository root
- [ ] `.github/copilot-instructions.md` exists

Done!

---

## If Auto-Detection Fails

If Copilot cannot auto-detect your project, use the manual version instead:
→ Open `SETUP_WORKFLOW_PROMPT.md` and follow the manual placeholder filling instructions.

---

## Quick Reference After Setup

Once setup is complete:
1. Read `QUICK_REFERENCE.md` for the 1-page cheat sheet
2. Use `SETUP_CHECKLIST.md` to verify everything was created
3. Follow `{ProjectName}/WORKFLOW.md` for task execution
4. Reference `.github/skills/README.md` for skill descriptions
