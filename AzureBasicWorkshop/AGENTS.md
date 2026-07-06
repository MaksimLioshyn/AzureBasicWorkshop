# Repository Instructions

## Scope
These instructions apply to the entire repository.

## Workflow Requirements for AzureOpenAIConsoleDemoWorkflow
When creating or modifying a class in the `AzureOpenAIConsoleDemoWorkflow` project, follow this two-phase workflow:

**Phase 1: Create Task**
1. Start from a structured task request using `task-intake`.
2. Call `create-task` to bootstrap the base production solution.
3. Keep the implementation small and reviewable.
4. Do not add XML documentation during bootstrap.

**Phase 2: Complete Task Workflow**
After bootstrap approval, invoke `complete-task-workflow` which runs:
1. Create placeholder test stubs for every public method.
2. Add English XML documentation (via `xml-doc-generator`).
3. Record changelog entries.
4. After final approval, expand tests to maximum practical coverage.

**General Requirements**
- Use the smallest matching skill from `.github/skills/` for each phase.
- Keep all documentation in English.
- Use the changelog format: `Task #N: modified method "MethodName"`.

## General Guidance
- Prefer small, focused changes.
- Keep test names aligned with the production method names.
- Preserve existing project structure unless a change is required by the task.
- If a new class is introduced, ensure the corresponding tests and documentation are introduced in the same change set whenever possible.
- Prefer the smallest matching skill from `.github/skills/` for the current task.
- Expand from placeholder tests to full coverage only after explicit approval.
