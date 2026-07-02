# Prompts

## 1. Title and Short Definition
A prompt is a one-shot natural-language request sent to an LLM. It is the most basic interaction unit.

## 2. Detailed Explanation
In simple terms, a prompt is what you type in chat. Technically, it is tokenized input processed by the model in the current context window.

Good prompts include:
- role
- context
- task
- output format
- constraints

## 3. Real-Life Analogy
A prompt is like a one-time cafe order: you ask, you get, and the next visit starts from scratch.

## 4. How It Works
```text
User input -> tokenization -> model inference -> generated response
```
Key methods:
- zero-shot
- few-shot
- chain-of-thought
- role prompting
- structured output prompting

## 5. Examples (2-3+)
```text
Bad: "Write something about Python"
Good: "Write a 200-word intro to Python for beginners, include history, strengths, use-cases"
```

```python
prompt = "Generate pytest unit tests for this function and include edge cases"
```

```json
{"task":"summarize","length":"150 words","style":"executive"}
```

## 6. Pros
1. Very fast start.
2. Maximum flexibility.
3. Great for exploration.
4. No infrastructure needed.
5. Useful for rapid prototyping.

## 7. Cons / Limits
1. No persistent behavior.
2. Inconsistent outputs.
3. No native access to live enterprise data.
4. Weak for long multi-step tasks.

## 8. When to Use / Not Use
Use when:
- one-off tasks
- brainstorming
- quick drafting

Do not use alone when:
- you need stable behavior
- you need tool/data access
- you need auditability and deterministic execution

## 9. Comparison with Adjacent Concepts
- Prompt vs Instruction: one-shot vs persistent.
- Prompt vs Skill: ad-hoc request vs reusable capability.

## 10. Speaker Notes
- Show bad-vs-good prompt examples.
- Emphasize: prompts remain foundational even in agent systems.

## 11. Audience Q&A
Q: Can prompt engineering alone solve production needs?
A: No. It helps quality, but production needs instructions, tools, and orchestration.

Q: Why do same prompts produce different outputs?
A: Stochastic decoding, context differences, and model settings.

Q: Is prompting still relevant with agents?
A: Yes. Agents still consume prompts internally.

*Back: 02_scheme.md | Next: 04_instructions.md*
