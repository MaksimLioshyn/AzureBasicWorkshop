# Class Generator Skill

## Purpose
Create a new production class with the smallest useful surface area.

## Output Requirements
- Create one class.
- Use a clear, stable name.
- Add English XML documentation comments for the class and every public method.
- Keep methods focused and minimal.
- Prefer deterministic, testable behavior.

## Required Workflow
1. Create the class.
2. Add public methods only when needed.
3. Document the class and methods in English.
4. Add a matching test class in the test project.
5. Record a changelog entry for each modified method.

## Cost-Saving Guidance
- Do not add extra helpers unless they reduce duplication.
- Do not introduce unrelated abstractions.
- Keep the class contract small so test generation is cheap.
- Use return values that are easy to validate in tests.

## Example Change Log Format
- `Task #1: modified method "GetSummary"`