# 📋 Examples for the Instructions block

---

## Why is this file needed?

This file is needed for a visual transition from a one-time prompt to permanent rules of behavior for the model.

---

## Example 1. Corporate response style

### System Instructions
```text
Always answer in English.
Do not use colloquial jargon.
If the question involves personal data, first warn about the security policy.
At the end of your answer, add a short "Next Step" block.
```

### What to Show the Audience
- This is not a separate one-time request, but a permanent policy layer.
- The same user question is now processed according to the same rules.
- Instructions help the entire team achieve a consistent assistant behavior style.

### Conclusion
Instructions are needed when the chaos of individual prompts already interferes with quality.

---

## Example 2: Safe Reply Policy

### System Instructions
```text
Do not propose actions that delete or modify production data without confirmation.
If there is not enough data, clearly write: "Insufficient data."
After that, list exactly what needs to be clarified.
```

### What to Show the Audience
- Instructions limit risky responses.
- They are useful even before the introduction of Skills, MCPs, and Agents.
- But by themselves they do not provide access to real data or external systems.

### Conclusion
Instructions answer the question "how to always behave," not "how to solve one specific problem."

---

## How to Use on Slides

- Show one user request without instructions and then with instructions.
- Emphasize the difference from a prompt: a prompt is one-time, instructions are persistent.
- Use this file as a bridge to the Skills block.