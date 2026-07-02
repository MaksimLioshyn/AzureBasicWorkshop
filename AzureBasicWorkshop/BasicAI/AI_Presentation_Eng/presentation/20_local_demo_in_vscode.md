# 🧭 Local Demo in VS Code / VS 2026

---

## Purpose of This File

This document is intended for a live demonstration of the full chain:

Prompts → Instructions → Skills → MCP → Agent → Workflow

It is written so that a clear demo scenario can be walked through locally, step by step.

---

## Short Answer to the Main Question

Yes, in general you understand the idea correctly:

1. First, show plain prompts.
2. Then extract stable rules from them and formalize as instructions.
3. Next, consolidate repeating logic into skills.
4. Then connect MCP, if you have an MCP-compatible client and a local or external MCP server.
5. After that, create an agent that uses instructions, skills, and tools.
6. Finally, show a workflow in which the agent is embedded as one of the steps.

One important clarification:

- **Prompts, Instructions, Skills, and Agent artifacts** can all be prepared locally in VS Code.
- **MCP** can also be demonstrated locally, but only if you have a working MCP server and a client/extension that can communicate with it.
- **Workflow** can also be demonstrated locally, most commonly as a GitHub Actions pipeline, task runner, orchestrator script, diagram-first scenario, or external pipeline.

---

## What to Actually Show in the Demo

### Demo Task Scenario
Use a single end-to-end task, for example:

"Prepare a response to a customer about their order status and escalate a complex case if necessary"

This works well because it lets you demonstrate all maturity levels using one task.

---

## Step 1. Show a Plain Prompt

### What to click in VS Code
1. Open the Copilot chat window.
2. Start a new chat.
3. Type a basic request manually.

### What to type
```text
Write a response to a customer about a 2-day order delay.
```

### What to explain to the audience
- This is the simplest level.
- It is fast, but unstable.
- One employee writes one prompt; another employee writes a different one.

---

## Step 2. Derive Instructions from Prompts

### What to click
1. In the Explorer, open the project root.
2. If the `.github` folder does not exist, create it.
3. Inside `.github`, create a file named `copilot-instructions.md`.

### What to add to the file
```markdown
# Instructions for the Support Assistant

- Always reply in English.
- Tone: polite and professional.
- Do not invent facts about the order.
- If there is insufficient information, state that explicitly.
- Always end with a "Next Step" block.
```

### What to explain
- Now we are not just making a one-off request.
- We have captured permanent behavioral rules.

---

## Step 3. Package Repeating Logic into a Skill

### What to click
1. In the Explorer, open `.github`.
2. Create a folder named `skills`.
3. Inside it, create a folder named `customer-reply`.
4. Inside `customer-reply`, create a file named `SKILL.md`.

### What to paste into `SKILL.md`
```markdown
---
name: customer-reply
description: 'Prepares a response to a customer about order status. Use when: customer support, order delay, shipment status, reply to customer.'
---

# Customer Reply

## When to Use
- A customer needs a reply about order status
- A polite delay notification needs to be sent

## Procedure
1. Determine whether there is enough data to reply
2. Formulate a brief apology
3. State the current status and new date, if known
4. Add a next step
```

### What to explain
- The prompt has been turned into a reusable module.
- This is no longer a one-time improvisation; it is a reusable capability.

---

## Step 4. Add a Second Skill

### What to click
1. In `.github/skills`, create a folder named `policy-check`.
2. Inside it, create `SKILL.md`.

### What to paste
```markdown
---
name: policy-check
description: 'Checks whether a reply violates corporate policy. Use when: compliance, support reply, policy validation.'
---

# Policy Check

## Procedure
1. Check that no personal data is exposed
2. Check that no fabricated facts are present
3. Confirm that the next step is stated explicitly
```

### What to explain
- Large skills are best split into narrowly focused ones.
- This is useful for both the agent and the workflow.

---

## Step 5. Connect MCP

### Important Note
This is the part that **can be shown locally but depends on your environment**.

### What is needed for the demo
1. An MCP-compatible client.
2. A configured MCP server.
3. A data source: a file, an API, a mock CRM, or GitHub.

### Safest demo option
- A read-only MCP pointed at a local JSON file containing order statuses;
- or a read-only MCP pointed at GitHub/files.

### What to explain
- A skill does not provide access to data.
- MCP does not define the response style or logic.
- MCP is responsible for access to real context.

---

## Step 6. Create an Agent

### What to click
1. In `.github`, create a folder named `agents`.
2. Inside it, create a file named `support-agent.agent.md`.

### What to paste
```markdown
---
name: support-agent
description: 'Customer support agent. Use for: support automation, order status, escalation, policy-aware replies.'
tools: []
---

# Support Agent

You handle customer inquiries about order status.

## Rules
- First determine whether there is enough data
- If tools are available, use them to verify facts
- Use the customer-reply skill for composing replies
- Use the policy-check skill before delivering the final reply
- If data is insufficient or the case is ambiguous, escalate to a human
```

### What to explain
- The agent coordinates steps and decides what to do next.
- It uses instructions, skills, and — when available — tools/MCP.

---

## Step 7. Connect to a Workflow

### What to show
There is no need to build complex infrastructure immediately. For a demo, showing one of the following formats is sufficient:

1. A YAML workflow;
2. diagram-as-code;
3. a simple Python orchestrator;
4. a GitHub Actions pipeline.

### Example workflow logic
```text
1. Receive a new inquiry
2. Check order data via MCP
3. Hand the task to support-agent
4. Run policy-check
5. If confidence is low -> escalate to a human
6. Otherwise send the reply
7. Log the result
```

### What to explain
- The workflow defines the skeleton of the process.
- The agent becomes one of the nodes, not the entire system.

---

## Can This Fit into a Single Demo?

Yes, if you focus on **the logic across one end-to-end case** rather than every implementation detail.

### Realistic timing
- Prompts: 2–3 minutes
- Instructions: 2 minutes
- Skills: 4–5 minutes
- MCP: 3–5 minutes
- Agent: 3–4 minutes
- Workflow: 3–4 minutes

Total: approximately 18–23 minutes for the demo portion.

This fits within the main presentation if:
- the slides carry only the concept and 1 key example;
- technical details are covered in this file rather than on the main slides.

---

## Practical Recommendation

For a live demo, do not try to show a production-grade MCP and a production workflow right away.

It is better to show:
1. one basic prompt;
2. one instructions file;
3. two small skills;
4. one safe read-only MCP;
5. one agent;
6. one compact workflow.

This keeps the demonstration clear, believable, and on schedule.
