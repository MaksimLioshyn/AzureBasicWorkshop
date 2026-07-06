# Workflow Setup Checklist

## Pre-Setup Verification

- [ ] Repository is initialized with Git
- [ ] Primary project created (e.g., `AzureOpenAIConsoleDemoWorkflow`)
- [ ] Test project created (e.g., `AzureOpenAIConsoleDemoWorkflow.Tests`)
- [ ] Build succeeds without errors
- [ ] Test framework installed (NUnit, xUnit, etc.)

## File Creation Checklist

### Repository Root
- [ ] Create `AGENTS.md`
  - [ ] Two-phase workflow structure documented
  - [ ] Phase 1: Create Task (bootstrap)
  - [ ] Phase 2: Complete Task Workflow (finish)
  - [ ] Changelog format specified: `Task #N: modified method "MethodName"`
  - [ ] General guidance on small, focused changes

### `.github` Directory
- [ ] Create `.github/copilot-instructions.md`
  - [ ] Project-specific conventions
  - [ ] Reference to two-phase workflow
  - [ ] Rules about test creation
  - [ ] English documentation requirement
  - [ ] Implementation style guidelines

### `.github/skills` Directory
- [ ] Create `.github/skills/README.md`
  - [ ] Skill index with descriptions
  - [ ] "When to use each skill" section
  - [ ] Cost-saving rules listed
  - [ ] Workflow order documented

- [ ] Create `.github/skills/task-intake.md`
  - [ ] Task # field
  - [ ] Title field
  - [ ] Description field
  - [ ] Acceptance Criteria field
  - [ ] Notes section

- [ ] Create `.github/skills/create-task.md`
  - [ ] Purpose: Bootstrap a task
  - [ ] Input format documented
  - [ ] Phase 1 workflow steps listed
  - [ ] Explicit note: "Do not add XML documentation during bootstrap"
  - [ ] Cost-saving guidance included

- [ ] Create `.github/skills/complete-task-workflow.md`
  - [ ] Purpose: Finish a bootstrapped task
  - [ ] Input requirements documented
  - [ ] Workflow order (4 skills):
	- [ ] test-stub-generator
	- [ ] xml-doc-generator
	- [ ] changelog-updater
	- [ ] coverage-expansion
  - [ ] Cost-saving rules listed

- [ ] Create `.github/skills/test-stub-generator.md`
  - [ ] Purpose documented
  - [ ] Create one test per public method
  - [ ] Naming convention: `{ClassName}Tests`
  - [ ] File naming convention: `{ClassName}Tests.cs`
  - [ ] False-resulting assertion example
  - [ ] NUnit-specific syntax (or your test framework)

- [ ] Create `.github/skills/xml-doc-generator.md`
  - [ ] Purpose: Add English XML documentation
  - [ ] Document class purpose
  - [ ] Document every public method
  - [ ] Example comment style provided
  - [ ] English-only requirement stated

- [ ] Create `.github/skills/changelog-updater.md`
  - [ ] Purpose: Record method changes
  - [ ] Required format: `Task #N: modified method "MethodName"`
  - [ ] One entry per modified public method
  - [ ] File location: `{ProjectName}/CHANGELOG.md`

- [ ] Create `.github/skills/coverage-expansion.md`
  - [ ] Purpose: Full test coverage phase
  - [ ] Only after approval rule
  - [ ] Add positive and negative tests
  - [ ] Cover edge cases and boundaries
  - [ ] Focus areas listed (success paths, error paths, null handling, boundaries)

### Project Directory (`{ProjectName}`)
- [ ] Create `{ProjectName}/WORKFLOW.md`
  - [ ] Goal documented
  - [ ] Two-phase structure explained
  - [ ] Phase 1 requirements (what to bootstrap)
  - [ ] Phase 2 requirements (which skills to run)
  - [ ] Test expectations
  - [ ] Documentation expectations
  - [ ] Changelog expectations

- [ ] Create `{ProjectName}/CHANGELOG.md`
  - [ ] Initialized with header "# Changelog"
  - [ ] First entry example (optional)

- [ ] Create example production class (optional but recommended)
  - [ ] Simple class with 2-3 public methods
  - [ ] English XML documentation added
  - [ ] File location: `{ProjectName}/{ExampleClassName}.cs`

- [ ] Create example test class (optional but recommended)
  - [ ] Test class with same naming convention
  - [ ] Test stubs for each method
  - [ ] False-resulting assertions
  - [ ] File location: `{ProjectName}.Tests/{ExampleClassName}Tests.cs`

## Content Verification Checklist

### AGENTS.md Verification
- [ ] References `{PROJECT_NAME}` correctly
- [ ] Two-phase workflow clearly separated
- [ ] Phase 1 does NOT include XML docs
- [ ] Phase 2 explicitly calls `xml-doc-generator`
- [ ] Changelog format is clear: `Task #N: modified method "MethodName"`
- [ ] General guidance on small changes

### Copilot Instructions Verification
- [ ] Phase 1 says "Do not add XML documentation during bootstrap"
- [ ] Phase 2 lists all 4 skills in order
- [ ] Test naming convention matches (e.g., `ClassNameTests`)
- [ ] English documentation requirement stated
- [ ] Implementation style guidelines present

### Skill Index (README.md) Verification
- [ ] All 8 skills listed and described
- [ ] When to use each skill explained
- [ ] Cost-saving rules section present
- [ ] Workflow order clear

### Create Task Skill Verification
- [ ] Explicitly states: "Do not add XML documentation during bootstrap"
- [ ] References "complete-task-workflow as next step"
- [ ] Minimal and focused implementation emphasized

### Complete Task Workflow Verification
- [ ] All 4 skills listed in correct order
- [ ] Prerequisites (bootstrap must be complete) stated
- [ ] Cost-saving rules present

### Test Stub Generator Verification
- [ ] Test class naming convention specified
- [ ] One test per public method rule
- [ ] False-resulting assertion shown
- [ ] Example test provided

### XML Doc Generator Verification
- [ ] English-only requirement stated
- [ ] Example summary format provided
- [ ] No private details documented rule

### Changelog Updater Verification
- [ ] Format rule: `Task #N: modified method "MethodName"`
- [ ] One entry per modified method
- [ ] File location specified

### Coverage Expansion Verification
- [ ] Approval gate clearly stated
- [ ] Focus areas listed (success, error, null, boundaries)
- [ ] Readability and determinism emphasized

### Project WORKFLOW.md Verification
- [ ] Two-phase structure explained
- [ ] Phase 1: Bootstrap without docs
- [ ] Phase 2: All 4 skills in order
- [ ] Test expectations documented
- [ ] Documentation expectations documented

## Build & Validation Checklist

- [ ] Repository builds without errors
- [ ] No compilation warnings in core files
- [ ] All .md files are valid Markdown
- [ ] Example class compiles successfully
- [ ] Example test class compiles successfully
- [ ] Example test runs and passes
- [ ] CHANGELOG.md exists and is parseable

## Documentation Verification

- [ ] All content written in English
- [ ] No hardcoded assumptions (uses placeholders where needed)
- [ ] All examples are valid syntax for the tech stack
- [ ] Links between documents are correct (e.g., references to skill files)
- [ ] File paths use forward slashes or correct OS convention

## Integration Checklist

- [ ] Git ignores generated files appropriately
- [ ] `.github/` folder structure is clean
- [ ] `skills/` folder contains all 8 files
- [ ] No duplicate skill files
- [ ] README.md in skills folder links to all skills

## Testing the System

- [ ] Create a test task using task-intake format
- [ ] Run create-task skill on test task
- [ ] Verify bootstrap phase produces:
  - [ ] Production class file
  - [ ] Test class file
  - [ ] No XML documentation
  - [ ] Minimal implementation
- [ ] Get approval (simulated)
- [ ] Run complete-task-workflow
- [ ] Verify Phase 2 produces:
  - [ ] Test stubs for each method
  - [ ] XML documentation on class and methods
  - [ ] Changelog entry updated
  - [ ] Ready for coverage expansion

## Final Verification

- [ ] All 8 skill files exist and are readable
- [ ] AGENTS.md is at repository root
- [ ] copilot-instructions.md is at `.github/`
- [ ] WORKFLOW.md is at project root
- [ ] CHANGELOG.md is at project root
- [ ] Build succeeds
- [ ] Team has been notified of new workflow
- [ ] Documentation is accessible to team
- [ ] First real task is ready to start

## Post-Setup Maintenance

- [ ] Schedule review of workflow after first 3 tasks
- [ ] Collect feedback from team
- [ ] Update skill descriptions based on learnings
- [ ] Document any tech-stack-specific customizations
- [ ] Create tip sheet for common issues
- [ ] Plan refresh cycle (quarterly or biannually)

## Sign-Off

- [ ] All checklist items completed
- [ ] Build verified
- [ ] Team trained (or documentation reviewed)
- [ ] System ready for first task
- [ ] Date: _______________
- [ ] Approved By: _______________

---

## Quick Stats

| Item | Count |
|------|-------|
| Total Files Created | 11 |
| Markdown Documents | 10 |
| Skill Files | 8 |
| Build Validation | 1 |
| Example Files | 2 (optional) |

## Notes

- Keep this checklist handy for future setup in other repositories
- Adapt skill content based on team feedback
- Update tech-stack specific sections when moving to different platforms
- Consider versioning the workflow system (e.g., v1.0, v1.1) for tracking changes
