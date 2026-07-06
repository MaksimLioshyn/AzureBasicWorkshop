# Copilot Instructions

## AzureOpenAIConsoleDemoWorkflow Conventions
When working in `AzureOpenAIConsoleDemoWorkflow`, follow the two-phase workflow:

**Phase 1: Create Task**
- Use `task-intake` to structure the task request.
- Call `create-task` to bootstrap the base solution.
- Do not add XML documentation during bootstrap.

**Phase 2: Complete Task Workflow**
- After approval, call `complete-task-workflow`.
- It invokes smaller skills in order:
  - test-stub-generator
  - xml-doc-generator
  - changelog-updater
  - coverage-expansion

**General Rules**
- Store Azure OpenAI settings in an app config file instead of hardcoded values in code.
- Create a test class whenever you create a production class.
- Add one placeholder test for every public method.
- Use false-resulting stubs for initial scaffolding.
- Write all documentation in English.
- Record changes as `Task #N: modified method "MethodName"`.
- Expand coverage only after approval.

## Implementation Style
- Keep generated code consistent with the existing project style.
- Prefer clear names over abbreviated names.
- Avoid leaving undocumented public members.
- Keep test scaffolding minimal until coverage expansion is requested.