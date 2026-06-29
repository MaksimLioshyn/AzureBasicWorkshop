# 🔌 Examples for the MCPs block

---

## Why is this file needed?

This file contains examples in a separate material where AI stops working only on text and starts using real data and tools.

---

## Example 1. Customer support + CRM

### Stream
1. The user enters the order number.
2. The assistant reads the status from the CRM via MCP.
3. Generates a response with the current delivery date.

### What to show the audience
- Before MCP, the model only assumes the answer.
- Following MCP, the answer is based on evidence.
- This is where access rights and call auditing are especially important.

### Conclusion
MCP is needed when AI must work not according to the model’s memory, but according to real information.

---

## Example 2. Dev Assistant + GitHub

### Stream
1. The assistant receives a list of open PRs via MCP.
2. Looks at statuses and blockers.
3. Generates a daily summary for the team.

### What to show the audience
- MCP links AI to the current state of the project.
- Without it, the summary would be pure fiction or manual copy-paste.
- MCP does a good job of showing the difference between “skill” and “access to reality.”

### Conclusion
Skills are responsible for the solution method, MCP is responsible for connecting to data and actions.

---

## A limitation worth saying out loud

Not every MCP can be raised in 5 minutes in a live demo. For a good demonstration it is better to take:
- file MCP;
- GitHub-like read-only MCP;
- a simple local server without write operations.
