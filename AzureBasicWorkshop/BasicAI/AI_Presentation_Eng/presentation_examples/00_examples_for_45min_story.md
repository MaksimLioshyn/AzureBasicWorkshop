# Examples of use for a 45-minute story

This file contains individual examples that can be used as practice bulletins during your presentation. Examples are grouped by maturity level.

## 1) Prompts

### Example 1. Draft letter to client
Situation: You need to quickly report a delivery delay.

Prompt:
"You are a customer support specialist. Write a short, polite letter to the client about the order being delayed by 2 days. Structure: 1) an apology, 2) a reason without technical details, 3) a new date, 4) compensation in the form of a 10% discount. Tone: businesslike, friendly."

Expected result: high-quality draft in 1-2 minutes.

### Example 2. Quick brief on the topic
Situation: An employee needs to quickly understand an unfamiliar topic.

Prompt:
"Explain the 'Model Context Protocol' topic to a new developer. Format: 5 abstracts, then 1 simple usage example, then 3 common mistakes."

Expected result: initial understanding without long research.

## 2) Instructions

### Example 1. Corporate response style
System instructions:
- Always answer in Russian.
- Don't use colloquial jargon.
- If the question concerns personal data, first warn about the security policy.
- At the end of the answer, add a short “Next step” block.

Effect: uniform assistant style for all employees.

### Example 2: Safe Reply Policy
System instructions:
- Do not propose actions to delete/change product data without confirmation.
- If there is not enough data, clearly write “Insufficient data” and pList what needs to be clarified.

Effect: Reducing the risk of risky responses and false confidence.

## 3) Skills

### Example 1. Skill "AI Code Review"
Input: diff pull request.
Output: list of comments by category:
- Security
- Correctness
- Maintainability

Effect: identical review structure and faster initial check.

### Example 2. Skill "Incident Summary"
Input: logs, error, time range.
Output:
- short reason,
- likely impact,
- 3 diagnostic steps,
- proposal for elimination.

Effect: a unified format for incident analysis for the duty team.

## 4) MCPs

### Example 1: Customer support + CRM
Stream:
1. The user enters the order number.
2. The assistant reads the status from the CRM via MCP.
3. Generates a response with the current delivery date.

Effect: answers based on facts, not assumptions.

### Example 2: Dev Assistant + GitHub
Stream:
1. The assistant receives a list of open PRs via MCP.
2. Generates a short daily summary of risks and blockers.

Effect: saving time on manually collecting statuses.

## 5) Agents

### Example 1. Competitive Analysis Agent
Goal: collect a review of 5 competitors in 20 minutes.
Agent steps:
1. Looks for sources.
2. Compares features.
3. Makes a table of differences.
4. Forms a conclusion and areas of uncertainty.

Effect: acceleration of research tasks.

### Example 2. Training plan selection agent
Goal: create a team training plan for 3 months.
Steps:
1. Analysis of the current stack.
2. Search for competency gaps.
3. Formation of a road map by week.

Effekt: quickly create an adaptive development plan.

## 6) Workflows

### Example 1. Workflow for processing requests
Steps:
1. Query classification.
2. Data extraction via MCP.
3. Generating a response.
4. Checking the confidence score.
5. Dispatch or escalate to operator.

Effect: stable SLA and process transparency.

### Example 2. Weekly job analytics workflow
Steps:
1. Collection of data from sources.
2. Normalization.
3. Quality check.
4. Report generation.
5. Publication and archive.

Effect: regular analytics on a schedule without manual chaos.

## 7) Antipatterns (for discussion)

### Antipattern 1. “Agent for everything”
Problem: rising costs, unpredictable behavior.
Countermeasure: agent only in complex nodes, the rest is workflow.

### Antipattern 2. Prompt-only in production
Problem: Lack of repeatability and quality control.
Countermeasure: add instructions + skills + quality metrics.

### Antipattern 3. Over-privileged MCP
Problem: risk of security incidents.
Countermeasure: least privilege + audit of all calls + approval for dangerous operations.

---

## Recommended timing for inserting examples
- Prompts/Instructions/Skills/MCP/Agents/Workflows blocks: 1 example in each block (approximately 1 minute per example)
- Case slides: 1 extended example each (1-2 minutes)
- In total, the examples give 10-12 minutes as part of a total 45-minute story.