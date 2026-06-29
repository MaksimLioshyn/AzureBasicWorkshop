# Code Analyzer Skill

Step 3a: First skill - code analysis

## Description

Skill for analyzing the structure of Python files and identifying potential improvements.

## How to use

In Copilot chat write:

```
@skill code-analyzer Analyze the build_presentation.py file and say:
1. Which functions perform which roles?
2. Where is optimization needed?
3. What variables can be made clearer?
```

## What skill does

1. Parses Python file
2. Defines functions and their purpose
3. Looks for error handling
4. Recommends code improvements
5. Offers refactoring

## Example result

```
📊 Analysis of the build_presentation.py file:

Features found: 8
- add_title_slide() - creating a title slide
- add_content_slide() - creating a content slide
- apply_gallery_theme() - applying the Gallery theme
- build() - main build function

Recommendations:
✓ Add type hints for function parameters
✓ Improve exception handling when working with files
✓ Split apply_gallery_theme() into a separate module
```

## Limitations

- Works only with Python files
- Maximum 100 lines of analysis at a time
- Requires correct Python syntax