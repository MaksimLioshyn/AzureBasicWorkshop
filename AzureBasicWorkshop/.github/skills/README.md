# Skill Index

These skills are intentionally small so future prompts can use only the minimum needed context.

## When to use each skill

### Two-Phase Workflow
- `task-intake.md` - submit a structured task request before work starts.
- `create-task.md` - bootstrap a task into a base implementation (phase 1).
- `complete-task-workflow.md` - finish a task using the smaller focused skills (phase 2).

### Focused Skills (used by complete-task-workflow)
- `class-generator.md` - create a new production class or update an existing one.
- `test-stub-generator.md` - create initial placeholder tests for each public method.
- `xml-doc-generator.md` - add or update English XML documentation comments.
- `changelog-updater.md` - record method-level changes in the required task format.
- `coverage-expansion.md` - replace placeholder tests with maximum practical coverage after approval.

## How to Use
1. Start with `task-intake.md` - fill in the task details.
2. Call `create-task.md` - bootstrap the base solution.
3. After approval, call `complete-task-workflow.md` - finish the task.

## Cost-saving rules

- Use one skill at a time.
- Keep the task scope narrow.
- Do not load unrelated project files.
- Prefer templates and fixed output formats.
- Expand from stub to full coverage only after explicit approval.

## Workflow order

1. Submit task using `task-intake.md`.
2. Create the task base using `create-task.md`.
3. Complete the task using `complete-task-workflow.md`.
4. Within completion, use these in order:
   - test-stub-generator
   - xml-doc-generator
   - changelog-updater
   - coverage-expansion
