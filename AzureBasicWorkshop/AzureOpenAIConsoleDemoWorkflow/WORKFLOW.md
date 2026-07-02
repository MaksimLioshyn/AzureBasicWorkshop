# AzureOpenAIConsoleDemoWorkflow Workflow

## Goal
This workflow defines how to create and update classes in `AzureOpenAIConsoleDemoWorkflow` so that production code, tests, documentation, and change logs stay aligned.

It supports a two-phase flow:
1. **Create Task** - bootstrap from a structured task request using `create-task`.
2. **Complete Task Workflow** - finish using smaller focused skills via `complete-task-workflow`.

## Required Process
When adding a new class or changing an existing class:

1. Create or update the production class.
2. Use the smallest applicable skill from `.github/skills/`.
3. Create or update a matching test class in `AzureOpenAIConsoleDemoWorkflow.Tests`.
4. Add a placeholder test for every public method.
5. Make the initial placeholder tests clearly incomplete and failing-safe, such as returning `false` or asserting a false condition.
6. Add XML documentation comments to the class and every public method.
7. Write all documentation comments in English.
8. Record the change using this format:
   - `Task #N: modified method "MethodName"`
9. After user approval, expand the tests to the maximum practical coverage.

## Test Expectations
- One test file per production class whenever practical.
- One stub test per public method at minimum.
- Add edge-case and negative tests during the coverage-expansion phase.
- Keep test names descriptive and aligned with the production method names.

## Documentation Expectations
- Document the purpose of the class.
- Document each public method with a short English summary.
- Keep comments concise and useful.

## Change Log Expectations
- Record every modified public method.
- Use a consistent task number.
- Keep the log entry in English.

## Example Change Log Entry
- `Task #3: modified method "CreateWorkflow"`