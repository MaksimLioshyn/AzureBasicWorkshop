# MCPs (Model Context Protocol)

## 1. Title and Short Definition
MCP is an open protocol that lets AI clients connect to external tools and data sources via a standard interface.

## 2. Detailed Explanation
MCP introduces a client-server model:
- MCP client inside AI host
- MCP servers exposing tools/resources/prompts
- standard request-response protocol

This removes one-off custom integrations for every assistant.

## 3. Real-Life Analogy
MCP is a USB port for AI systems: one standard, many compatible devices.

## 4. How It Works
```text
User goal -> AI decides tool use -> MCP call -> tool executes -> result returned -> AI continues
```

## 5. Examples (2-3+)
```json
{"method":"tools/call","params":{"name":"read_file","arguments":{"path":"README.md"}}}
```

```json
{"mcpServers":{"filesystem":{"command":"npx","args":["@modelcontextprotocol/server-filesystem"]}}}
```

```python
# custom MCP server tool
Tool(name="get_customer", description="Fetch customer profile")
```

## 6. Pros
1. Standardized integration model.
2. Live access to real enterprise data.
3. Better portability across AI hosts.
4. Clearer security boundaries at server level.

## 7. Cons / Limits
1. Additional infrastructure to operate.
2. Security model must be carefully designed.
3. Ecosystem maturity still evolving.

## 8. When to Use / Not Use
Use when:
- assistant needs live systems
- tools must be shared across assistants
- you want protocol-level standardization

Do not use when:
- one-off local prompting is enough
- no external tools/data required

## 9. Comparison with Adjacent Concepts
- MCP vs Skill: access layer vs capability layer.
- MCP vs Agent: tool provider vs autonomous decision-maker.

## 10. Speaker Notes
- Show one real MCP config and one tool call flow.
- Emphasize least-privilege access design.

## 11. Audience Q&A
Q: Is MCP the same as function calling?
A: Similar intent, but MCP is an open cross-host protocol.

Q: Can MCP be read-only?
A: Yes, and read-only is recommended by default.

Q: Is MCP enterprise-ready?
A: Yes, with proper auth, authorization, and monitoring.

*Back: 05_skills.md | Next: 07_agents.md*
