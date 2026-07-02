# Create Task Skill

## Purpose
Bootstrap a task from a structured request into a base implementation.

## Input Format
Use the task intake format:
- Task #
- Title
- Description
- Acceptance Criteria

## Workflow
When the task is first received:
1. Create the base production solution.
2. Add the required wiring, setup, and minimal scaffolding.
3. Keep the implementation small and reviewable.
4. Prepare the code so the completion workflow can finish it cleanly.

## Output Requirements
- Produce the base solution first.
- Keep bootstrap minimal and focused.
- Do not expand coverage before approval.
- Use English for documentation and log entries.

## Next Step
After bootstrap is complete and approved, invoke `complete-task-workflow`.

## Cost-Saving Guidance
- Keep bootstrap minimal.
- Use only the smallest matching skill for each subtask.
- Do not re-read unrelated files.
- Keep task scope aligned to the acceptance criteria.

## Example Invocation
- `Create task 12: OrderProcessor from the task intake.`
