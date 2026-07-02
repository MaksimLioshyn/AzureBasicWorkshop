# XML Documentation Generator Skill

## Purpose
Add English XML documentation comments to production code.

## Output Requirements
- Document the class with a concise English summary.
- Document every public method.
- Keep comments short and specific.
- Match the code style already used in the file.

## Required Workflow
1. Identify every public member.
2. Add an XML summary for the class.
3. Add XML summaries for each public method.
4. Keep all wording in English.
5. Avoid documenting private implementation details.

## Cost-Saving Guidance
- Write only the comments needed for public contracts.
- Reuse wording when method behavior is similar.
- Do not add narrative comments that repeat the code.

## Example Comment Style
- `<summary>Returns a short description of the workflow.</summary>`