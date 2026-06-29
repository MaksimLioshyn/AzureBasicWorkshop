# 🔄 Examples for the Workflows block

---

## Why is this file needed?

This file is needed to clearly explain how a deterministic process wraps AI components and makes them more reliable.

---

## Example 1. Workflow for processing requests

### Steps
1. Query classification.
2. Data extraction via MCP.
3. Generating a response.
4. Checking the confidence score.
5. Dispatch or escalate to operator.

### What to show the audience
- Not only intelligence is important here, but also a controlled route.
- Workflow defines what happens when there is an error, low confidence, and exceptions.
- This format is convenient for production processes and SLAs.

### Conclusion
Workflow is needed when control, auditing and disaster recovery are important.

---

## Example 2. Weekly job analytics workflow

### Steps
1. Data collectionfrom sources.
2. Normalization.
3. Quality check.
4. Report generation.
5. Publication and archive.

### What to show the audience
- One agent could do this flexibly, but less predictably.
- Workflow makes releases regular and controlled.
- Individual workflow nodes can still use prompts, skills, MCP and agents.

### Conclusion
Workflow does not cancel AI, but disciplines its use in the real process.