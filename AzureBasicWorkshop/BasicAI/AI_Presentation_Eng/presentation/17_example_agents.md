# 🤖 Examples for the Agents block

---

## Why is this file needed?

This file helps show where a normal tool call ends and the agent cycle begins: goal, plan, action, check, adjust.

---

## Example 1. Competitive analysis agent

### Goal
In 20 minutes, collect a review of 5 competitors.

### Agent Steps
1. Looks for sources.
2. Compares features.
3. Makes a table of differences.
4. Forms a conclusion and areas of uncertainty.

### What to show the audience
- The agent does not just respond once, but goes through a cycle of actions.
- He can choose the next step based on the results of the previous one.
- This is stronger than prompt and skill, but more difficult to control.

### Conclusion
The agent is justified where the course of action is not fully known in advance.

---

## Example 2. Training plan selection agent

### Goal
Create a team training plan for 3 months.

### Steps
1. Analysis of the current stack.
2. Search for competency gaps.
3. Formation of a road map by week.

### What to show the audience
- The agent works well in adaptive research tasks.
- But it requires restrictions on steps, cost and set of tools.
- It is useful for the agent to be given Skills and MCP as working primitives.

### Conclusion
A strong agent almost always relies on instructions, skills and tools.
