# Test Stub Generator Skill

## Purpose
Create initial placeholder tests for every public method in a production class.

## Output Requirements
- Create one test class per production class.
- Create one test stub per public method.
- Keep the initial test stubs intentionally incomplete.
- Use a false-resulting assertion or equivalent placeholder.

## Required Workflow
1. Inspect the public methods on the production class.
2. Create a matching test class.
3. Add one stub test per public method.
4. Make the stubs easy to replace with real assertions later.
5. Expand coverage only after approval.

## Cost-Saving Guidance
- Keep the tests thin and direct.
- Do not write broad setup code unless it is shared by multiple tests.
- Use method names that mirror the production API.
- Avoid large fixture graphs for initial scaffolding.

## Example Placeholder Pattern
- `Assert.That(result, Is.False);`