# Workflows

## 1. Title and Short Definition
A workflow is a predefined process where steps and transitions are explicitly controlled, while AI may be used in selected nodes.

## 2. Detailed Explanation
Workflows prioritize:
- predictability
- auditability
- recoverability
- operational reliability

They are ideal for recurring business processes.

## 3. Real-Life Analogy
A workflow is a manufacturing line or a recipe: sequence is fixed, quality gates are explicit.

## 4. How It Works
```text
Trigger -> Step1 -> Step2 -> Branch -> Approval/Retry -> Finalize
```

AI appears as bounded functions in specific steps.

## 5. Examples (2-3+)
```yaml
steps:
  - ingest
  - classify_with_ai
  - validate
  - route
  - notify
```

```python
if quality_score < threshold:
    route_to_human_review()
```

```text
Hybrid: Workflow shell + Agent inside one exploratory step
```

## 6. Pros
1. Strong reliability and process control.
2. Easier monitoring and incident response.
3. Better compliance and audit trail.
4. Clear retries/checkpoints.

## 7. Cons / Limits
1. Less flexible than agents.
2. Higher design effort upfront.
3. Must be maintained as process changes.

## 8. When to Use / Not Use
Use when:
- process is repeatable and regulated
- quality and reliability dominate
- high throughput is required

Do not use when:
- task is highly exploratory
- sequence cannot be predesigned

## 9. Comparison with Adjacent Concepts
- Workflow vs Agent: deterministic reliability vs adaptive autonomy.
- Workflow vs Skill: orchestration layer vs capability module.

## 10. Speaker Notes
- Emphasize production concerns: retries, checkpointing, audit logs.
- Promote hybrid architecture for best balance.

## 11. Audience Q&A
Q: Can workflows include agents?
A: Yes, this is a common hybrid pattern.

Q: Are workflows always no-code?
A: No, many are code-first or mixed.

Q: Why not use agents everywhere?
A: Cost, control, and compliance often require deterministic flow.

*Back: 07_agents.md | Next: 09_comparison.md*
