# 🎯 Examples for the Skills block

---

## Why is this file needed?

This file shows how a reusable skill is obtained from a set of repeated prompts and rules.

---

## Example 1. Skill "AI Code Review"

### Login
- diff pull request;
- if necessary, a list of changed files;
- corporate review rules.

### Exit
- notes on categories:
  - Security
  - Correctness
  - Maintainability

### What to show the audience
- Previously, each developer formulated his own prompt for review.
- Then the team stabilized the format through Instructions.
- This was then packaged into a separate skill that can be called again.

### Conclusion
Skill turns good practice into a standard team module.

---

## Example 2. Skill "Incident Summary"

### Login
- logs;
- error;
- time range;
- brief context of the incident.

### Exit
- brief reason;
- likely impact;
- 3 diagnostic steps;
- proposal for elimination.

### What to show the audience
- The Skill is especially useful in on-duty engineering practice.
- It reduces the variation in quality between different engineers.
- This skill is already closer to the team’s operating library than to just a smart prompt.

### Conclusion
Skills are needed where a task is repeated and must be performed equally well.

---

## Mini-scenario for transition from prompt to skill

1. The team finds a successful prompt.
2. Adds standing instructions.
3. Fixes execution steps and response format.
4. Packs it into `SKILL.md`.
5. Uses the skill repeatedly in different tasks.
