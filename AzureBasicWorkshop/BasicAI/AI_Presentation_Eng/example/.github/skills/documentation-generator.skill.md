# Documentation Generator Skill

Step 3b: Second skill - documentation generation

## Description

Skill for automatically generating documentation based on code and comments.

## How to use

In Copilot chat write:

```
@skill documentation-generator Create a README for the presentation/ folder
based on the files that are there
```

## What skill does

1. Scans all files in a folder
2. Extracts headers and structure
3. Analyzes files by type (.md, .py, .json)
4. Generates a structured README
5. Adds Table of Contents
6. Creates use cases

## Example result

```
# Presentation documentation

## Structure

### Folder: presentation/
- 01_introduction.md - Introduction to the topic
- 02_architecture.md - Solution architecture
- 03_prompts.md - Details about Prompts
...

## How to use this documentation

1. Read introduction.md for general context
2. Study the individual components in files 03-09
3. Look at the examples in presentation_examples/

## Quick links

- [Basic concepts](03_prompts.md)
- [Code examples](../presentation_examples/)
```

## Options

- `--format`: markdown (default), html, pdf
- `--depth`: scan depth (default 2 levels)
- `--include-code`: whether to include code blocks (true/false)