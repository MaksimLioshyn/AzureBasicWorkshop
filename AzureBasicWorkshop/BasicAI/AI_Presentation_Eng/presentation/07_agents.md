# Agents

## 1. Title and Short Definition
An agent is an AI system that autonomously plans and executes multi-step actions to reach a goal.

## 2. Detailed Explanation
Agents combine:
- reasoning
- memory/state
- tool use
- iterative feedback loop

They are useful when the exact sequence is unknown in advance.

## 3. Real-Life Analogy
An agent is a self-managed specialist who receives a goal, decides steps, and reports results.

## 4. How It Works
```text
Perceive -> Plan -> Act -> Observe -> Reflect -> Repeat -> Final result
```

Controls typically include max iterations, budget limits, and approval checkpoints.

## 5. Examples (2-3+)
```python
agent.run("Analyze competitors and prepare a report")
```

```text
ReAct loop:
Thought -> Action(tool) -> Observation -> Thought
```

```markdown
AGENTS.md policy:
- allowed tools
- restricted actions
- human approval triggers
```

## 6. Pros
1. Handles non-trivial multi-step tasks.
2. Adapts to intermediate outcomes.
3. Strong productivity for research/ops scenarios.
4. Can orchestrate skills and tools dynamically.

## 7. Cons / Limits
1. Non-deterministic behavior.
2. Risk of loops and incorrect plans.
3. Higher inference cost.
4. Harder debugging and governance.

## 8. When to Use / Not Use
Use when:
- task is exploratory
- steps are not fully known
- adaptation is required

Do not use when:
- deterministic process is mandatory
- compliance requires strict reproducibility

## 9. Comparison with Adjacent Concepts
- Agent vs MCP: agent decides, MCP executes tools.
- Agent vs Workflow: adaptive autonomy vs deterministic sequence.

## 10. Speaker Notes
- Explain guardrails: limits, approvals, logs.
- Position agents as part of a controlled architecture, not magic automation.

## 11. Audience Q&A
Q: Are agents safe for production?
A: Yes, with strict boundaries, observability, and approvals for sensitive actions.

Q: Why are agents expensive?
A: They can trigger many model/tool steps per task.

Q: Can agents be deterministic?
A: Not fully. For strict determinism, wrap them in workflows.

*Back: 06_mcps.md | Next: 08_workflows.md*
