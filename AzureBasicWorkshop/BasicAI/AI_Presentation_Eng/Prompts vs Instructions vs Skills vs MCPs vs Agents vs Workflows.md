# Instructions for Creating the Presentation: "Prompts vs Instructions vs Skills vs MCPs vs Agents vs Workflows"

## Goal
Create a set of files with detailed material for a presentation that can be used to fully prepare for a talk and work with the topic. Each file = one section with extensive content.

---

## File Structure
Create the following files in the `presentation/` folder:

```
presentation/
├── 00_overview.md        # General overview and navigation
├── 01_introduction.md    # Introduction
├── 02_scheme.md          # Overview diagram
├── 03_prompts.md         # Prompts
├── 04_instructions.md    # Instructions
├── 05_skills.md          # Skills
├── 06_mcps.md            # MCPs
├── 07_agents.md          # Agents
├── 08_workflows.md       # Workflows
├── 09_comparison.md      # Comparison table
├── 10_when_to_use.md     # When to use what
├── 11_cases.md           # Examples and cases
└── 12_conclusion.md      # Conclusion
```

---

## General Requirements for Each File

Each concept file (03-08) MUST contain the following sections:

1. **Title and brief definition** (1-2 sentences)
2. **Detailed explanation** (plain language + technical)
3. **Real-life analogy** (for easy understanding)
4. **How it works** (mechanics, diagram, stages)
5. **Code/usage examples** (minimum 2-3 examples)
6. **Pros** (minimum 4 points with explanations)
7. **Cons / limitations** (minimum 3 points)
8. **When to use / when NOT to use**
9. **Comparison with neighboring concepts** (how it differs)
10. **Speaker notes** (what to emphasize in the talk)
11. **Possible audience questions + answers** (3-5 questions)

**Volume:** each concept file — no less than 300-400 lines of quality material.

**Style:** clear, with examples, no fluff, technically accurate.

---

## Step-by-Step Execution Plan

### STEP 1: Create `00_overview.md`
Contents:
- Table of contents for all files with brief descriptions
- Main idea of the presentation (1 paragraph)
- "Complexity ladder": visual diagram of the transition from Prompts to Workflows
- How to use the material

### STEP 2: Create `01_introduction.md`
Cover:
- Context: how interaction with AI evolved (brief history)
- Problem: why a single prompt is not enough
- Spectrum of approaches: simplicity vs autonomy/power axis
- Key thesis: these are not competitors, but levels of a single system
- What the audience will learn by the end

### STEP 3: Create `02_scheme.md`
Cover:
- Text visualization of the overall scheme (ASCII diagram or description)
- Brief definition of each of the 6 concepts (1 sentence)
- How the concepts build upon each other
- Table "level -> name -> essence"

### STEP 4: Create `03_prompts.md`
Use GENERAL REQUIREMENTS (sections 1-11). Special attention:
- The concept of "one-shot" requests
- Prompting techniques (zero-shot, few-shot, chain-of-thought)
- Examples of good and bad prompts
- Analogy: a one-time order at a cafe

### STEP 5: Create `04_instructions.md`
Use GENERAL REQUIREMENTS. Special attention:
- System prompt vs regular prompt
- Persistence of behavioral rules
- System prompt examples
- Analogy: employee job description
- Clear distinction from Prompts

### STEP 6: Create `05_skills.md`
Use GENERAL REQUIREMENTS. Special attention:
- Concept of modularity and reusability
- How a skill is packaged
- Examples: analysis skill, formatting skill
- Analogy: professional competencies
- Distinction from Instructions and MCPs

### STEP 7: Create `06_mcps.md`
Use GENERAL REQUIREMENTS. Special attention:
- What is Model Context Protocol (standard from Anthropic)
- Architecture: MCP client-server
- Connection examples (files, databases, APIs, GitHub)
- Analogy: USB port for AI
- Difference from Skills (data access vs capabilities)

### STEP 8: Create `07_agents.md`
Use GENERAL REQUIREMENTS. Special attention:
- Agent work cycle: perception -> planning -> action -> reflection
- Autonomy and decision-making
- Use of tools
- Examples: research agent, task coordinator
- Analogy: an independent employee
- Difference from Workflows (freedom vs rigid scenario)
- Risks: hallucinations, loops, control

### STEP 9: Create `08_workflows.md`
Use GENERAL REQUIREMENTS. Special attention:
- Rigidly defined sequence of steps
- Determinism and predictability
- Examples: request processing, ETL processes
- Analogy: a conveyor belt / recipe
- Agents vs Workflows comparison (the key distinction!)
- Hybrid approach: workflow with agents inside

### STEP 10: Create `09_comparison.md`
Contents:
- Large comparison table by criteria:
  - Setup complexity, Autonomy, Flexibility, Predictability, Cost, Required technical skills, Typical use cases
- Radar chart or text description of strengths/weaknesses
- Matrix "when each one wins"

### STEP 11: Create `10_when_to_use.md`
Contents:
- Decision tree (in text): "If..., then use..."
- KISS principle: don't over-engineer without reason
- How concepts combine (stack examples)
- Anti-patterns: typical selection mistakes

### STEP 12: Create `11_cases.md`
Contents:
- 3 detailed real-world cases
- For each: task -> solution using different approaches -> conclusion
- Evolution case: one task from prompt to agent
- Results comparison

### STEP 13: Create `12_conclusion.md`
Contents:
- Summary of key ideas
- Main message: levels, not competitors
- Development trends (where the industry is heading)
- Practical recommendations "where to start"
- List of sources for further study

---

## Quality Criteria (check after completion)

- [ ] All 13 files created
- [ ] Each concept file contains all 11 mandatory sections
- [ ] Concrete code/prompt examples in each relevant file
- [ ] Analogies are understandable to an unprepared person
- [ ] Differences between neighboring concepts are clearly articulated
- [ ] Speaker notes added
- [ ] Technical terms explained at first mention
- [ ] Material is logically connected between files

---

## Execution Order
1. Create files strictly in sequence (00 -> 12)
2. After each file, briefly report what was done
3. At the end, perform a self-check against the checklist
4. Suggest improvements if you notice gaps

---

## Formatting
- Use `##`, `###` headings for structure
- Use tables for comparisons
- Use code blocks for examples
- Add emojis for navigation (in moderation)
- Highlight **important** things in bold
