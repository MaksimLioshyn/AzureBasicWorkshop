# Instructions

## 1. Title and Short Definition
Instructions are persistent rules that define how AI should behave across requests.

## 2. Detailed Explanation
Instructions are usually system-level context. They set role, tone, boundaries, and policy constraints.

Typical structure:
- role
- scope
- style
- do and do-not rules
- escalation behavior

## 3. Real-Life Analogy
Instructions are a job description and operating handbook for an employee.

## 4. How It Works
```text
System instructions loaded first -> user prompt processed under those rules -> response generated
```
Instruction precedence is typically above user-level requests.

## 5. Examples (2-3+)
```text
You are a support assistant for Product X.
Always answer in English, concise, and policy-compliant.
If unknown, say "I don't know" and suggest escalation.
```

```markdown
# .github/copilot-instructions.md
- Use Python 3.12
- Add type hints
- Add tests for edge cases
- Never hardcode secrets
```

```yaml
system_message: |
  You are an HR assistant.
  Use only approved internal policy documents.
```

## 6. Pros
1. Consistent behavior.
2. Better safety and governance.
3. Team-level standardization.
4. Easier review/versioning via git.

## 7. Cons / Limits
1. Can become rigid if over-constrained.
2. Must be maintained over time.
3. Does not provide data/tool access by itself.

## 8. When to Use / Not Use
Use when:
- building team or product assistants
- enforcing tone and policy
- requiring consistency

Do not use alone when:
- task depends on live external data
- you need autonomous planning

## 9. Comparison with Adjacent Concepts
- Instruction vs Prompt: persistent global behavior vs one request.
- Instruction vs Skill: behavior policy vs executable task capability.

## 10. Speaker Notes
- Show one same prompt with two different instruction sets.
- Explain policy and compliance value for enterprises.

## 11. Audience Q&A
Q: How long should instructions be?
A: Usually short and clear beats long and vague.

Q: Can users override instructions?
A: Sometimes they may try; robust systems add guardrails and validation.

Q: Are instructions enough for production?
A: No. You still need tools, monitoring, and process control.

*Back: 03_prompts.md | Next: 05_skills.md*
