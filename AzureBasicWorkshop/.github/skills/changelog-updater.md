# Changelog Updater Skill

## Purpose
Record method-level code changes in a consistent, compact format.

## Output Requirements
- Log each modified public method.
- Use the required task format.
- Keep the log entry in English.

## Required Format
- `Task #N: modified method "MethodName"`

## Required Workflow
1. Identify the modified public methods.
2. Write one entry per method.
3. Keep the task number consistent within a change set.
4. Use the same formatting every time.

## Cost-Saving Guidance
- Do not include long summaries.
- Do not duplicate unchanged methods.
- Keep the log entry separate from implementation details.

## Example
- `Task #2: modified method "IsReadyForCoverageExpansion"`