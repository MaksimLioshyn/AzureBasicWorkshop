# Complete Task Workflow Skill

## Purpose
Finish a bootstrapped task by invoking the smaller focused skills in sequence.

## Input Requirements
- The bootstrap phase must be complete and approved.
- The production class or solution structure is ready.
- Task # and context are clear.

## Workflow Order
Execute these skills sequentially after bootstrap approval:
1. `test-stub-generator.md` - create placeholder tests for each public method.
2. `xml-doc-generator.md` - add English XML documentation.
3. `changelog-updater.md` - record method-level changes.
4. `coverage-expansion.md` - expand tests to maximum practical coverage.

## Output Requirements
- Keep the completion phase separate from bootstrap.
- Use the smallest matching skill for each subtask.
- Do not re-read unrelated files between subtasks.
- Use English for all documentation and log entries.

## Cost-Saving Guidance
- Execute one skill at a time.
- Keep task scope aligned to the acceptance criteria.
- Do not speculate about additional requirements.
- Wait for approval before expanding coverage.

## Example Invocation
- `Complete task 12 using the complete-task-workflow.`
- `Run complete-task-workflow for OrderProcessor.`
