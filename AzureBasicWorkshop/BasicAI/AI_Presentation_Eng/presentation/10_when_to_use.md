# When to Use What

## Decision Tree

```text
Is it a one-off task?
  yes -> Prompt
  no  -> Need consistent behavior?
            yes -> add Instructions
            no  -> Need external tools/data?
                      yes -> add MCPs
                      no  -> Is capability reusable?
                                yes -> add Skills
                                no  -> Multi-step with unknown sequence?
                                          yes -> Agent
                                          no  -> Workflow
```

---

## KISS Principle

Use the smallest architecture that solves the problem reliably.
Do not jump to agents/workflows before proving value with prompts.

---

## Practical Stack Examples

### Stack A: Team coding assistant
- Instructions
- Skills (review, tests, docs)
- MCPs (repo/files)

### Stack B: Competitive intelligence
- Instructions
- MCP web/search tools
- Agent for adaptive research

### Stack C: Support ticket operations
- Workflow orchestration
- AI classification node
- human review for risky cases

---

## Common Anti-Patterns

1. Agent for everything.
2. No instructions in production assistants.
3. Single giant skill for all tasks.
4. MCP with over-privileged access.
5. Prompt-only architecture for regulated processes.

---

## Quick Checklist

- Repeatability needed?
- Live data required?
- Determinism required?
- Audit trail required?
- Human approvals needed?
- Budget and latency constraints?

---

## Speaker Notes

- Walk through one real audience use case on this slide.
- Reinforce incremental evolution strategy.

---

*Back: 09_comparison.md | Next: 11_cases.md*
