# Skills

## 1. Title and Short Definition
A skill is a reusable, task-focused capability module the AI can apply repeatedly.

## 2. Detailed Explanation
Skills package domain know-how in a form that can be triggered by context or explicit invocation.

A skill usually includes:
- activation criteria
- procedure/checklist
- expected output format
- quality criteria

## 3. Real-Life Analogy
A skill is a professional competency: code review, debugging, documentation, risk scoring.

## 4. How It Works
```text
Task arrives -> system selects relevant skill -> skill procedure is applied -> structured output returned
```

## 5. Examples (2-3+)
```markdown
Skill: code-review
- Check logic bugs
- Check security
- Check tests
- Return findings by severity
```

```python
def run_skill_error_analysis(error, context):
    return llm_call(template="error-analysis", data={"error": error, "context": context})
```

```yaml
skill: cv-review
steps: [parse, evaluate, score, report]
```

## 6. Pros
1. Strong reuse.
2. Team standardization.
3. Easier testing and governance.
4. Better consistency of output structure.

## 7. Cons / Limits
1. Initial setup overhead.
2. Ongoing maintenance needed.
3. Possible overlap/conflicts between skills.

## 8. When to Use / Not Use
Use when:
- tasks repeat often
- you need stable quality pattern
- domain expertise should be codified

Do not use when:
- task is one-off
- data access is the core need (use MCP)

## 9. Comparison with Adjacent Concepts
- Skill vs Instruction: capability module vs global behavior policy.
- Skill vs MCP: knows how to perform vs knows how to access data/tools.

## 10. Speaker Notes
- Demonstrate one skill file and one execution example.
- Highlight modularity and ownership by domain teams.

## 11. Audience Q&A
Q: Is a skill just a function?
A: Not exactly. A function is deterministic code; a skill is AI-guided procedural capability.

Q: Can multiple skills be chained?
A: Yes, commonly in agents and workflows.

Q: Who should author skills?
A: Domain experts with AI engineer review.

*Back: 04_instructions.md | Next: 06_mcps.md*
