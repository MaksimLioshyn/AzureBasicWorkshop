# Examples for Live Demo: 5 Steps

This folder contains ready-made examples to demonstrate the progression from a simple `prompt` to a full `workflow`.

## 📋 Folder structure

```
example/
├── step-1-basic-prompt.txt ← Step 1: Simple prompt
├── .github/
│ ├── copilot-instructions.md ← Step 2: Global instructions
│ ├── skills/
│ │ ├── code-analyzer.skill.md ← Step 3a: First skill
│ │ └── documentation-generator.skill.md ← Step 3b: Second skill
│ └── agents/
│ └── support-agent.agent.md ← Step 4: Agent for support
└── step-5-workflow-diagram.md ← Step 5: Workflow diagram
```

## 🎯 How to use in demo

### Step 1: Basic Prompt (5 minutes)
1. Open Copilot Chat (`Ctrl+Shift+I`)
2. Copy the prompt from `step-1-basic-prompt.txt`
3. Show how Copilot responds without instructions
4. **Conclusion**: basic functionality, but without project context

### Step 2: Add Instructions (5 minutes)
1. Create a file `.github/copilot-instructions.md`
2. Copy the content from the example
3. Ask the same question in chat
4. Show the difference in the answer (Russian language, practical examples)
5. **Conclusion**: Instructions improve the quality of answers

### Step 3: Connect Skills (5 minutes)
1. Create a folder `.github/skills/`
2. Add two skill files from the examples
3. Update `copilot-instructions.md` to mention skills
4. Ask a question like: `@skill code-analyzer Analyze the file...`
5. **Conclusion**: Skills add specialized capabilities

### Step 4:Connect Agent (3 minutes)
1. Create `.github/agents/support-agent.agent.md`
2. Copy the content from the example
3. Call agent: `@support-agent Explain to me...`
4. Show how the agent uses skills and instructions
5. **Conclusion**: Agent coordinates all components

### Step 5: Showing the Workflow Diagram (2 minutes)
1. Open `step-5-workflow-diagram.md`
2. Show Mermaid a data flow diagram
3. Explain how each layer interacts
4. **Conclusion**: This is a complete system of interaction between components

## ⏱️ Total: 20 minutes of demonstration

- Step 1: 5 min (Prompt)
- Step 2: 5 min (Instructions)
- Step 3: 5 min (Skills)
- Step 4: 3 min (Agent)
- Step 5: 2 min (Workflow)

## 💡 Key points to demonstrate

1. **Evolution**: from simple to complex
2. **Synergy**: each level improves the previous one
3. **Practicality**: all examples work in VS Code
4. **Automation**: workflow makes the system smart and responsive

## 🚀 Additional tips

- During the demo, you can open VS Code and show files in real time
- Questions from students can be used as natural examples
- Show how each component affects the quality of answers
- Emphasize the difference between “just Chat” and “integrated system”

## 📚 Link to presentation

These examples correspond to the **"Local Demo: Step-by-Step Scenario" slide from the main presentation.

All concepts were presented on slides:
- Slide 3-4: Basic Concepts
- Slide 9-16: Examples by level
- Slide 25: Local demonstrationwalkie-talkie

This folder is ready-made practical material for conducting a live demo.