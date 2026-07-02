# Coverage Expansion Skill

## Purpose
Replace placeholder tests with maximum practical coverage after approval.

## Output Requirements
- Expand the existing test class.
- Add positive and negative tests where appropriate.
- Cover edge cases and boundary conditions.
- Keep test logic aligned with the production contract.

## Required Workflow
1. Start from the placeholder tests.
2. Review the public surface of the class.
3. Add behavior-focused test cases.
4. Add edge-case coverage.
5. Keep the tests readable and deterministic.

## Cost-Saving Guidance
- Expand only after explicit approval.
- Reuse shared setup where it actually reduces duplication.
- Avoid speculative tests for unsupported behavior.
- Keep the test matrix small and relevant.

## Example Focus Areas
- success paths
- invalid input paths
- null handling
- boundary values