# 🎭 Real-World Cases and Examples

---

## 🧩 Case 1: Customer Request Processing at a Bank

### Task
Bank "FinancePro" receives 5,000 requests per day via email, chat, and a website form. Operators spend 3–5 minutes on each one. The goal is to automate processing while maintaining quality.

---

### Solutions via Different Approaches

#### Option A: Prompts Only (anti-pattern)
```
Operator manually copies the request → ChatGPT → copies the response

Result:
✗ Every operator writes their own prompt differently
✗ Inconsistent response quality
✗ No CRM integration
✗ No audit trail
✗ 5,000 manual operations per day

Time saved: ~0%
```

#### Option B: Instructions + MCPs (basic automation)
```python
# Instructions: define the behavior of the support bot
SYSTEM = """
You are an assistant at FinancePro Bank.
You help with: cards, loans, payments, deposits.
Tone: polite, formal. Language: English.
For complex operational questions: redirect to 1-800-555-0100.
Never reveal account balances without verification.
"""

# MCP: connect CRM and FAQ knowledge base
mcps = [
    CRMMCPServer(connection=crm_db),      # Customer history
    FAQMCPServer(kb_path="./knowledge"),   # Knowledge base
    TicketMCPServer(jira_url="...")        # Ticket creation
]

# Result: AI responds with customer history and knowledge base context
Time saved: ~40% of operator time (auto-handling of simple requests)
```

#### Option C: Workflow + Instructions + MCPs (production-ready)
```
WORKFLOW: ticket-processing-pipeline

Step 1: TRIGGER
  New request from email/chat/form → queue

Step 2: ENRICHMENT (MCP)
  Fetch: customer history from CRM, product types, account status

Step 3: CLASSIFICATION (AI + Instructions)
  Categories: billing, technical, complaint, info_request
  Urgency: urgent / normal / low

Step 4: ROUTING (code, deterministic)
  billing → finance team queue
  complaint → VIP handling
  urgent → skip queue, immediate

Step 5: RESPONSE GENERATION (AI + Skills)
  Apply skill: customer-support-reply
  Use: customer history + FAQ + templates

Step 6: QUALITY CHECK (code)
  Response length: 50–300 words
  No personal data in the email body
  Includes links to specific FAQ articles

Step 7: HUMAN REVIEW (human, for complex cases)
  If complexity_score > 0.7 → human
  Otherwise → auto-send

Step 8: SEND + LOG (MCP)
  Send response → update CRM → analytics

Result: 85% of requests handled automatically
Time saved: ~70% of operator time
Audit trail: every step is logged
```

---

### Case 1 Summary

| Approach | Automation | Quality | Reliability | Implementation Cost |
|----------|-----------|---------|-------------|---------------------|
| Prompts Only | 0% | Unstable | None | Minimal |
| Instructions + MCP | 40% | Good | Medium | Medium |
| **Workflow + everything** | **85%** | **Excellent** | **High** | High |

**Conclusion:** For a large-scale business problem, investing in the full stack is justified.

---

## 🧩 Case 2: Code Review in a Development Team

### Task
A startup of 20 developers wants to improve code quality through AI-assisted review of every PR. Currently, reviews take 30–60 minutes and are frequently delayed.

---

### Solution Evolution: From Prompt to Automation

#### Stage 1 (Day 1): Prompt Experiment
```
Developer Ivan copies code into ChatGPT:
"Review this code"

Result: It works! But everyone writes their own prompt → different aspects get checked
```

#### Stage 2 (Week 1): Instructions + Skills in Copilot
```markdown
# .github/copilot-instructions.md

You are a code reviewer at Startup X.

Code standards:
- Python 3.12, FastAPI, Pydantic v2
- Tests: pytest, minimum 80% coverage
- Security: OWASP Top 10
- Documentation: Google-style docstrings

Review format:
- Critical: blocks merge (bugs, security)
- Major: must be fixed before merge
- Minor: can go in a follow-up PR
- Nit: cosmetic, optional
```

```markdown
# SKILL.md — code-review

## Review checklist (always check):
1. Logic errors and edge cases
2. SQL Injection, XSS, CSRF
3. Error handling (except Exception: pass → ❌)
4. Data types (type hints everywhere)
5. Tests: are there tests for new functionality?
6. Performance: N+1 queries, unnecessary joins

## Output format:
```
### 🔴 Critical
- [SECURITY] Line X: SQL injection via f-string
  Fix: use parameterized queries

### 🟡 Major
- [TESTS] Function Y has no test coverage
  
### 🟢 Minor
- [STYLE] Line Z: too long (>88 characters)
```
```

**Stage 2 result:** Everyone writes reviews consistently; Copilot assists directly inside VS Code.

#### Stage 3 (Month 1): Workflow in CI/CD (automation)
```yaml
# .github/workflows/ai-code-review.yml
name: AI Code Review

on:
  pull_request:
    types: [opened, synchronize]

jobs:
  ai-review:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0
      
      - name: Get Changed Files
        id: diff
        run: |
          git diff origin/main...HEAD --name-only > changed_files.txt
          git diff origin/main...HEAD > full_diff.txt
      
      - name: AI Review via API
        run: |
          python scripts/ai_review.py \
            --diff full_diff.txt \
            --skill code-review \
            --instructions .github/copilot-instructions.md \
            --output review_result.json
      
      - name: Post Review Comments
        uses: actions/github-script@v7
        with:
          script: |
            const review = require('./review_result.json');
            await github.rest.pulls.createReview({
              pull_number: context.payload.pull_request.number,
              body: review.summary,
              comments: review.inline_comments,
              event: review.has_critical ? 'REQUEST_CHANGES' : 'COMMENT'
            });
```

**Stage 3 result:** Every PR receives a review in 2 minutes. Critical issues are blocked automatically.

---

### Final Comparison for Case 2

| Stage | Review Time | Coverage | Quality | Effort |
|-------|-------------|---------|---------|--------|
| Before AI | 45 min | ~60% of PRs | Inconsistent | High |
| Stage 1 (Prompt) | 15 min | 100% | Unstable | Medium |
| Stage 2 (Instructions+Skills) | 5 min | 100% | Good | Low |
| **Stage 3 (Workflow)** | **2 min** | **100%** | **Excellent** | **Minimal** |

---

## 🧩 Case 3: Evolution of the "Labor Market Vacancy Analysis" Task

### Task
An HR director wants to understand what is happening in the IT job market: which skills are in demand, how salaries have changed, and which companies are hiring most actively.

**Key question: how does the solution evolve from a prompt to an agent?**

---

### Version 1.0: A Single Prompt (Day 1)
```
HR Director: "ChatGPT, what do companies currently require from Python developers?"

ChatGPT: "Typically they require knowledge of Django, FastAPI, PostgreSQL..."

Problem: Data is from training (outdated). No up-to-date information.
```

### Version 2.0: Prompt + MCP for Search (Day 7)
```python
# Add MCP for real-time search
mcps = [BraveSearchMCP()]

# Now the AI searches for current data
"Find the latest 10 Python Developer job postings in the market
 and analyze the requirements. Focus on skills and salaries."

Result: Current data! But only 10 vacancies, still manual work.
```

### Version 3.0: Agent with MCPs (Month 1)
```python
# Agent autonomously researches the market
research_agent = ReActAgent(
    instructions="""
    You are a labor market analyst.
    You collect vacancy data and analyze trends.
    Cite your sources. Be objective.
    """,
    tools=[
        web_search_mcp,      # Search for vacancies
        file_write_mcp,      # Save data
        python_executor_mcp  # Analysis and statistics
    ]
)

task = """
Analyze the Python Developer job market for the past month:
1. Collect data from job boards (e.g., LinkedIn, Indeed, Glassdoor)
2. Identify the top 10 most in-demand skills
3. Determine the median salary range
4. Find the top 5 most actively hiring companies
5. Compare with the previous quarter
6. Provide a forecast for the next quarter
Save the report to /reports/labor_market_q4_2024.md
"""

result = research_agent.run(task)
# The agent decides: which sites to search, how to parse, how to analyze
# Takes 15–20 minutes, makes 30–50 LLM calls
```

### Version 4.0: Workflow (automated weekly report)
```python
# Workflow for regular execution (every Monday at 8:00)

weekly_market_report = Workflow(
    name="weekly-labor-market-analysis",
    schedule="0 8 * * MON",
    steps=[
        Step("collect_data",
             tool=web_scraper_mcp,
             params={"sources": ["linkedin.com", "indeed.com", "glassdoor.com"],
                     "query": "Python Developer",
                     "date_range": "last_7_days"}),
        
        Step("process_data",
             skill=data_analysis_skill,
             instruction="Extract: skills, salaries, companies, experience levels"),
        
        Step("compare_with_history",
             tool=db_mcp,
             params={"query": "SELECT * FROM market_data ORDER BY week DESC LIMIT 4"}),
        
        Step("generate_insights",
             llm_call=True,
             instruction_file="hr-analyst-instructions.md",
             prompt="Identify trends and changes over the past week"),
        
        Step("create_report",
             skill=report_generation_skill,
             output_format="markdown"),
        
        Step("notify",
             tool=email_mcp,
             params={"to": "hr-director@company.com",
                     "subject": "Weekly Report: IT Labor Market"})
    ]
)
```

---

### Version Comparison for Case 3

| Version | Data Freshness | Depth | Automation | Cost |
|---------|---------------|-------|------------|------|
| 1.0 Prompt | ❌ Outdated | Minimal | 0% | $ |
| 2.0 MCP | ✅ Current | Shallow | 0% | $ |
| 3.0 Agent | ✅ Current | Deep | 50% (HR triggers manually) | $$$ |
| **4.0 Workflow** | ✅ **Current** | **Deep** | **100%** | **$$** |

**Final takeaway for this case:** Evolving from simple to complex is the right strategy. Start with a prompt, discover the real needs, then add layers only as required.

---

## 🔑 Key Lessons from the Cases

### Lesson 1: Start Small
> None of the successful cases started with a Workflow. Always begin with a prompt, validate the value, then add complexity.

### Lesson 2: The Right Tool for Scale
> Prompts are good for one-off tasks. Workflows are for thousands. Agents are for non-standard situations within a Workflow.

### Lesson 3: Don't Be Afraid to Combine
> The best solutions use multiple layers: Instructions define behavior, MCPs provide data, Skills standardize logic, Workflows ensure reliability.

### Lesson 4: Monitoring Is Mandatory
> In every case, the production solution included logging and monitoring. AI without monitoring is a black box.

### Lesson 5: Human-in-the-Loop Where Risk Is High
> Banking case: complex cases → human. DevOps case: destructive actions → human. This is not a weakness — it is the correct architecture.

---

## 🎤 Speaker Notes

- **Bank case** — familiar to corporate audiences. Request processing is a pain point for many companies.
- **Code review case** — directly relevant for IT audiences. Many already use Copilot.
- **Evolution case** — shows the "hero's journey": from a naive prompt to the right architecture. The audience sees the progression.
- You can invite the audience to share **their own cases** from practice.
- Emphasize: **none of the cases use all 6 levels without reason** — only what is actually needed.
- Time for this slide: **10 minutes**

---

*← [10_when_to_use.md](10_when_to_use.md) | [12_conclusion.md](12_conclusion.md) →*