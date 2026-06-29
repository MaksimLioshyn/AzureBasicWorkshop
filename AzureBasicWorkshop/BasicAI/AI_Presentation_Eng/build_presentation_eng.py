from pathlib import Path
import subprocess
from pptx import Presentation
from pptx.util import Pt

ROOT = Path(r"d:\Study\AI Presentation SEFE\Prompts vs Instructions vs Skills vs MCPs vs Agents vs Workflows\AI_Presentation_Eng")
OUT = ROOT / "Prompts vs Instructions vs Skills vs MCPs vs Agents vs Workflows.pptx"
THEME = Path(r"C:\Program Files\Microsoft Office\root\Document Themes 16\Gallery.thmx")


def set_notes(slide, notes_text):
    if not notes_text:
        return
    notes_frame = slide.notes_slide.notes_text_frame
    notes_frame.clear()
    notes_frame.text = notes_text


def set_title_font(shape, size=42):
    p = shape.text_frame.paragraphs[0]
    for run in p.runs:
        run.font.name = "Calibri"
        run.font.bold = True
        run.font.size = Pt(size)


def set_body_font(tf, size=22):
    for p in tf.paragraphs:
        for run in p.runs:
            run.font.name = "Calibri"
            run.font.size = Pt(size)


def add_title_slide(prs, title, subtitle, notes=None):
    slide = prs.slides.add_slide(prs.slide_layouts[0])
    slide.shapes.title.text = title
    set_title_font(slide.shapes.title, 40)

    sub = slide.placeholders[1]
    sub.text = subtitle
    set_body_font(sub.text_frame, 22)
    set_notes(slide, notes)


def add_content_slide(prs, title, bullets, notes=None):
    slide = prs.slides.add_slide(prs.slide_layouts[1])
    title_shape = slide.shapes.title
    title_shape.text = title
    set_title_font(title_shape, 34)

    body = slide.placeholders[1]
    tf = body.text_frame
    tf.clear()
    for i, b in enumerate(bullets):
        if i == 0:
            tf.text = b
        else:
            p = tf.add_paragraph()
            p.text = b
            p.level = 0
    set_body_font(tf, 20)
    set_notes(slide, notes)


def educational_note(terms, mechanics, example, limits, conclusion):
    return (
        f"Terms: {terms}\n\n"
        f"Mechanics: {mechanics}\n\n"
        f"Practical example: {example}\n\n"
        f"Limitations: {limits}\n\n"
        f"Conclusion: {conclusion}"
    )


def apply_gallery_theme(pptx_path):
    if not THEME.exists():
        print(f"Theme file not found: {THEME}")
        return

    command = (
        "$pptPath = '{ppt}'; "
        "$themePath = '{theme}'; "
        "$pp = New-Object -ComObject PowerPoint.Application; "
        "$pres = $pp.Presentations.Open($pptPath,$false,$false,$false); "
        "$pres.ApplyTemplate($themePath); "
        "$pres.Save(); "
        "$pres.Close(); "
        "$pp.Quit();"
    ).format(
        ppt=str(pptx_path).replace("'", "''"),
        theme=str(THEME).replace("'", "''"),
    )

    result = subprocess.run(
        ["powershell", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command", command],
        capture_output=True,
        text=True,
    )
    if result.returncode != 0:
        error_text = (result.stderr or result.stdout).strip()
        print(f"Gallery theme apply failed: {error_text}")


def build():
    prs = Presentation()

    add_title_slide(
        prs,
        "Prompts vs Instructions vs Skills vs MCPs vs Agents vs Workflows",
        "How to design AI systems: from a single request to full automation",
        notes=educational_note(
            "AI architecture — the organization of layers, data, and rules. Maturity — the degree of controllability and reproducibility.",
            "This presentation is structured as a 45-minute story: 5 minutes for the opening frame, about 30 minutes for the maturity levels, 8 minutes for cases, and 2 minutes for consolidation. The core principle: we do not choose one technology — we design a combination of layers for the task. First we define the goal of the process and the cost of failure, then we select the minimum sufficient level, and then we incrementally add control and reliability tools. It is important to keep in focus that moving between layers is a managed evolution, not a sudden replacement of everything.",
            "An internal team started with prompts for quick tasks, then added policies, integrations, and a workflow as requirements grew.",
            "Risk 1: overloading the audience with terms without connecting logic. Risk 2: discussing capabilities without operational constraints. Risk 3: drilling into tool details and losing the criteria for architectural decisions. The 45-minute format also requires timing discipline — spending too long on basic definitions leaves no time for cases and applied solutions.",
            "The main goal of the session is to give a repeatable method for architectural decisions that can be applied immediately after the presentation."
        ),
    )

    add_content_slide(
        prs,
        "The Complexity Ladder",
        [
            "Prompts -> Instructions -> Skills -> MCPs -> Agents -> Workflows",
            "Left: simpler and faster to start",
            "Right: higher autonomy and reliability",
        ],
        notes=educational_note(
            "Autonomy — the ability to perform actions without manual control. Reliability — predictability of the result when repeated.",
            "The complexity ladder defines the trade-off axis. On the left is a low startup threshold and high experiment speed; on the right is higher operational discipline and control. The story for this slide is a route: prompts cover quick launch, instructions provide a consistent behavior standard, skills scale repeatable expertise, MCP connects real data and actions, agents add adaptive planning, workflows ensure a stable operational process. After each subsequent slide it is useful to return to the ladder and show at which level the discussed pattern sits.",
            "For one-off text generation, prompts are enough; for regular customer support with SLA, you need a workflow with integrations.",
            "If moving right is seen as mandatory for every case, the team gets over-engineering and rising costs. If stuck on left-side levels when quality requirements are high, operational risk appears. Organizational maturity is also a constraint: the further right the layer, the more demands on process owners, monitoring, and change management.",
            "The ladder is not for maximizing complexity, but for making a justified choice of the next layer based on the task's requirements."
        ),
    )

    add_content_slide(
        prs,
        "Prompts",
        [
            "A one-shot text request to AI",
            "Pros: speed, flexibility, minimal entry barrier",
            "Cons: no stability and no access to live data",
            "Best for quick one-off tasks",
        ],
        notes=educational_note(
            "Prompt — a text task statement to the model. Zero-shot/few-shot — execution without examples or with examples.",
            "The mechanics of the prompt approach are simple: the user provides context, goal, constraints, and desired format. Quality increases when the request is structured: model role, input data, success criteria, format, and constraints. One case shows this: first the request 'Write a letter to a customer about a delay', then the improved version 'You are a support specialist. Write a short polite letter about a 2-day order delay, state the new date and compensation', then a few-shot with one successful example. This shows how result quality can be significantly improved without changing the model. In workflows, prompts often serve as a quick pre-stage for drafts and hypotheses.",
            "A support specialist gets a draft response to a customer about a delivery delay in one minute and edits it for context.",
            "The prompt approach is limited in tasks requiring strict repeatability, auditing, and policy control. Responses can be unstable between runs. No built-in access to current internal data without integrations. At scale, quality also heavily depends on the individual prompt engineering skill of each team member.",
            "Prompts are effective as a fast start and prototyping layer, but are rarely sufficient for production use."
        ),
    )

    add_content_slide(
        prs,
        "Instructions",
        [
            "Persistent behavioral rules for the model (system prompt)",
            "Ensure consistent style and response boundaries",
            "Important for corporate assistants",
            "Do not replace data access — that is the role of MCP",
        ],
        notes=educational_note(
            "Instructions — system-level rules that apply on top of user requests. Guardrails — protective constraints on model behavior.",
            "The mechanics of instructions is a centralized policy layer. Here you define required tone, response format, prohibitions, handling of sensitive topics, and security rules. In practice, this reduces the quality variance between team members and makes assistant behavior predictable. A clear example: the same request 'Reply to the customer about their order' without instructions produces arbitrary style, but with rules 'reply formally, no jargon, do not invent order status, end with a Next Step block' produces a consistent corporate response. It is critical to explain that instructions are not a one-time setting but a managed artifact with versioning and regular review.",
            "The internal HR assistant always replies formally, does not disclose PII, and in disputed questions refers to approved policy.",
            "Instructions do not provide access to live data and do not replace integrations. Rules that are too strict reduce usefulness and response variety; rules too loose bring back chaos. Many conflicting rules without priorities cause inconsistent model behavior. There is also a risk of policy becoming outdated if there is no owner and update process.",
            "Instructions provide behavioral stability and a base control layer, but for real value they need to be combined with data and processes."
        ),
    )

    add_content_slide(
        prs,
        "Skills",
        [
            "Reusable competency modules",
            "Examples: code review, error analysis, report formatting",
            "Pros: standardization and scaling",
            "Require maintenance and versioning",
        ],
        notes=educational_note(
            "Skill — a formalized module for solving a repeatable task. Versioning — controlled update of behavior.",
            "The mechanics of skills starts with identifying repeatable task classes. Each skill gets a clear contract: input, processing steps, quality criteria, and expected output. Skills then form a library of team competencies and are used as building blocks in different processes. A clear example: the team first manually asks AI for a code review, then defines a standard format for Security, Correctness, and Maintainability findings and formalizes it as a separate skill used by all developers. A good practice is to assign a skill owner and regularly verify its relevance.",
            "In an engineering team, separate skills handle diff review, analyze build errors, and generate structured release notes.",
            "The key limitation is the tendency to create 'giant' skills with multiple roles at once, which breaks testability and maintenance. Without versioning, updates cause unexpected regressions in processes. Without a catalog and usage rules, users lose track of which skill to choose. Skills also require continuous maintenance as product components.",
            "Skills scale team expertise when managed with discipline and clear ownership boundaries."
        ),
    )

    add_content_slide(
        prs,
        "MCPs",
        [
            "Model Context Protocol: the standard for connecting tools",
            "Provides access to files, databases, APIs, GitHub",
            "Analogy: a USB port for AI",
            "Requires a strict access control model",
        ],
        notes=educational_note(
            "MCP — the protocol for connecting the model to external tools. Least privilege — the minimum sufficient access rights.",
            "MCP mechanics: the model makes a tool call, receives actual data or executes a permitted action, then builds a response on real context. For the audience, the request lifecycle is important: request -> rights check -> operation execution -> data return -> logging. This layer moves AI from guessing mode to working with facts. In the 45-minute story, MCP deserves more time because this is where security and operational questions arise most often.",
            "A support assistant reads order status from the CRM and composes a personalized customer response based on current data.",
            "MCP limitations are primarily about security: over-privileged access, no audit trail, mixing test and production channels. The second group of limitations is external service reliability: API unavailability must have fallback logic. Third is governance: without clear read/write rules and approval for critical operations the system becomes vulnerable. Also important to account for the cost and maintenance of integrations.",
            "MCP makes AI truly applied, but requires a mature access model, monitoring, and fault tolerance."
        ),
    )

    add_content_slide(
        prs,
        "Agents",
        [
            "Autonomous cycle: perception -> plan -> action -> reflection",
            "Strong for non-standard multi-step tasks",
            "Risks: loops, hallucinations, cost",
            "Need limits, logs, and human-in-the-loop",
        ],
        notes=educational_note(
            "Agent — an autonomous executor of a chain of steps toward a goal. Human-in-the-loop — mandatory human involvement in critical actions.",
            "Agent mechanics are built on the cycle: task analysis, planning, executing actions via tools, checking results, and adjusting the plan. A clear example: the agent is given the goal 'compile a review of five competitors', after which it searches for sources, identifies comparison criteria, builds a difference table, and separately marks zones of uncertainty. It is important to present the agent as a controlled system, not a 'self-directed intelligence with no limits'. Mandatory constraints here are concrete: step limit, cost limit, timeout, list of prohibited actions, and logging. This sets the right expectations in the audience and reduces the risk of improper deployment.",
            "An agent prepares a competitive review: gathers data from multiple sources, aligns criteria, and produces an analytical summary.",
            "Agent limitations include the possibility of looping, unnecessary tool calls, and rising costs. Insufficient source verification can lead to incorrect conclusions. Without human-in-the-loop, critical operations become risky. Testing agent scenarios is harder than workflows, so telemetry and incident analysis requirements are higher. In organizations with low process maturity, the agent layer may create more risks than value.",
            "Agents provide flexibility for complex tasks but are safe only within a strict managed control boundary."
        ),
    )

    add_content_slide(
        prs,
        "Workflows",
        [
            "Deterministic sequence of steps",
            "Pros: predictability, auditability, recoverability",
            "Better for regular business processes",
            "Hybrid: workflow shell + agent on complex steps",
        ],
        notes=educational_note(
            "Workflow — a formally defined process route. SLA — target metrics for time, quality, and availability.",
            "Workflow mechanics: the process is split into sequential stages with transition conditions, retry policy, checkpoints, and escalation. Unlike the agent approach, the key advantage is predictability and auditability. A clear example: a customer request goes through classification, data reading via MCP, response generation, confidence score check, and then either auto-send or escalation. It is immediately clear where a delay occurred, where there is an error, and where manual processing is needed. The hybrid variant adds an agent only in nodes that require adaptive decisions, keeping the overall deterministic framework.",
            "Request processing: request classification -> data extraction -> response generation -> policy check -> send or escalate.",
            "Limitations: an overly rigid workflow handles non-standard cases poorly if there are no exception branches. Complex process graphs are hard to maintain without visualization and an owner model. Without per-stage metrics it is impossible to understand the causes of SLA degradation. Also, frequent business rule changes require constant workflow updates, otherwise the process becomes formally stable but practically outdated.",
            "Workflow is the foundational layer for production reliability, and maximum value comes from combining it with adaptive nodes."
        ),
    )

    add_title_slide(
        prs,
        "Examples by Level",
        "From a single prompt to workflow and anti-patterns",
        notes=educational_note(
            "Demo block — a series of short practical scenes connecting theory to real actions.",
            "This block is embedded after the main definitions so the audience first understands the levels and then immediately sees them in examples. The order is the same as in the main maturity ladder: first prompt, then instructions, skills, MCP, agent, workflow, and typical selection mistakes.",
            "One end-to-end scenario shows the evolution of customer support from a one-time text to a managed process.",
            "Showing all examples before explaining the concepts turns the material into disconnected fragments. Showing them too late makes the theory abstract.",
            "The right place for examples is immediately after the concepts and before the overall comparison matrix."
        ),
    )

    add_content_slide(
        prs,
        "Example: Prompts",
        [
            "Situation: notify a customer about an order delay",
            "Basic prompt: 'Write a letter to the customer about the order delay'",
            "Improved prompt: role, deadline, structure, tone, compensation",
            "Conclusion: prompt is good for a quick draft but does not guarantee a consistent standard",
        ],
        notes=educational_note(
            "Prompt — a one-shot text task without a persistent rules layer.",
            "The most visual demonstration here is to compare two requests side by side. First: 'Write a letter to the customer about an order delay'. Second: 'You are a customer support specialist. Write a short polite letter about a 2-day order delay. Structure: apology, reason without technical details, new date, 10 percent compensation'. The audience immediately sees how structure improves the result quality.",
            "This example works especially well in a live Copilot chat because the difference is visible instantly.",
            "Even a good prompt does not solve the problem of uniform rules for the team and does not connect real order data.",
            "Prompt is the best starting point for testing an idea, but not the final process architecture."
        ),
    )

    add_content_slide(
        prs,
        "Example: Instructions",
        [
            "The same request, but now with persistent behavioral rules",
            "Rules: English language, formal tone, do not invent order status",
            "Every response ends with a 'Next Step' block",
            "Conclusion: instructions stabilize the response style and policy",
        ],
        notes=educational_note(
            "Instructions — system rules that act on top of user requests.",
            "In practice, show the same request 'Reply to the customer about the order', first without system rules, then with instructions: 'Always answer in English. Do not use jargon. Do not invent order status. End with a Next Step block'. The response becomes predictable and corporate in form.",
            "This is the clearest way to show that instructions do not replace the prompt, but discipline its use.",
            "Instructions on their own do not fetch facts from CRM and do not create a reusable skill module.",
            "This level is especially important for general-purpose assistants and corporate copilot scenarios."
        ),
    )

    add_content_slide(
        prs,
        "Example: Skills",
        [
            "Repeatable logic can be packaged into a skill",
            "Example: skill 'AI Code Review' with Security, Correctness, Maintainability categories",
            "Example 2: skill 'Incident Summary' for on-call team",
            "Conclusion: a skill turns a successful practice into a reusable module",
        ],
        notes=educational_note(
            "Skill — a formalized module for a repeatable task with a clear input, procedure, and output format.",
            "A useful example: the team first manually asks AI to review a diff, then defines a standard checklist and comment format, and formalizes it into SKILL.md as 'AI Code Review'. Second example — 'Incident Summary', where logs and an error are the input and a brief root cause, impact, and diagnostic steps are the output.",
            "This shows the audience how a successful prompt plus instructions produces a library-level skill.",
            "If everything is packed into one skill, it becomes hard to maintain and loses clear ownership boundaries.",
            "Skills are especially useful where the team wants consistent quality and fast onboarding of new members."
        ),
    )

    add_content_slide(
        prs,
        "Example: MCP",
        [
            "Scenario 1: customer support + CRM via MCP",
            "Scenario 2: Dev Assistant + GitHub summary of open PRs",
            "Main idea: AI gets real facts instead of guessing",
            "Conclusion: MCP handles data and action access, not response style",
        ],
        notes=educational_note(
            "MCP — the protocol through which the model connects to tools and external data sources.",
            "First clear example: the user enters an order number, the assistant reads the status from CRM via MCP and forms a response with the current delivery date. Second example: the assistant reads the list of open pull requests via MCP and compiles a short daily summary of blockers. In both cases the model no longer relies only on its internal memory.",
            "This helps the audience clearly separate the concepts of skill and MCP: one defines the way to work, the other provides real context.",
            "The most common mistake here is giving the model too broad access rights or not logging tool calls.",
            "MCP makes AI applied, but this is exactly where security and governance requirements grow the most."
        ),
    )

    add_content_slide(
        prs,
        "Example: Agents",
        [
            "Competitive analysis agent: 5 competitors in 20 minutes",
            "Steps: find sources, compare criteria, build difference table, draw conclusions",
            "Learning plan agent: analyze stack, find gaps, produce week-by-week roadmap",
            "Conclusion: an agent is justified where the action order cannot be fully fixed in advance",
        ],
        notes=educational_note(
            "Agent — an autonomous executor that chooses the next step toward a set goal within constraints.",
            "The clearest example is a research agent: given the task of compiling a review of five competitors, it goes through a cycle of finding sources, identifying comparison criteria, building a table, and formulating conclusions. Another example: an agent that builds a 3-month team learning plan based on the current stack and competency gaps.",
            "These examples clearly show why an agent needs both instructions and skills and, when available, MCP.",
            "Without step limits, budget controls, and an action log, the agent quickly becomes an expensive and risky component.",
            "An agent is useful not everywhere, but where adaptability genuinely provides value."
        ),
    )

    add_content_slide(
        prs,
        "Example: Workflows",
        [
            "Request workflow: classify -> MCP -> response -> confidence -> send/escalate",
            "Job market workflow: collect data -> normalize -> quality check -> report -> archive",
            "Main idea: AI nodes are embedded in a managed route",
            "Conclusion: workflow is needed where SLA, auditability, and error recovery matter",
        ],
        notes=educational_note(
            "Workflow — a deterministic process skeleton into which AI is embedded as one or more steps.",
            "First example: a request-processing pipeline where classification comes first, then data reading via MCP, then response generation, confidence score check, and either auto-send or human escalation. Second example: weekly job market analytics where collection, normalization, quality control, and publishing stages run on a schedule.",
            "This shows the audience that workflow does not compete with the agent but frames it where needed.",
            "An overly rigid workflow without exception branches handles non-standard cases poorly and becomes outdated quickly.",
            "In production scenarios, workflow most often becomes the foundation of reliability and observability."
        ),
    )

    add_content_slide(
        prs,
        "Example: Anti-patterns",
        [
            "'Agent for everything' -> expensive and unstable",
            "Prompt-only in production -> no repeatability",
            "Over-privileged MCP -> security incident risk",
            "Conclusion: correct architecture is determined by the task, not the trendiness of the tool",
        ],
        notes=educational_note(
            "Anti-pattern — a typical solution that initially looks convenient but systematically degrades architecture.",
            "The clearest set for this slide: the team wants an 'agent for everything' and faces rising costs; the team keeps only prompts in production and loses quality control; the team gives MCP excessive rights and gets incident risk. After that, immediately give the countermeasure: workflow for standard steps, instructions and skills for stability, least privilege for tools.",
            "This slide works especially well before the comparison and selection block because the audience already understands not only the capabilities but also the cost of mistakes.",
            "If anti-patterns are only shown at the end, listeners have often already mentally chosen an overly complex solution and then resist simplification.",
            "Good architecture starts not with maximum power, but with the discipline of selection and constraints."
        ),
    )

    add_content_slide(
        prs,
        "Concept Comparison",
        [
            "Prompts: simple, but less predictable",
            "Instructions/Skills: balance quality and repeatability",
            "MCPs: connect real data and actions",
            "Agents: flexibility. Workflows: reliability",
        ],
        notes=educational_note(
            "Predictability — stability of result for the same input. Combined architecture — multiple layers working together.",
            "The comparison mechanics is built through shared criteria: startup speed, control level, data access, cost of error, maintenance cost. Each layer covers different requirements, so evaluating 'better/worse' without the process context is incorrect. In the 45-minute structure, this slide acts as a synthesis: it connects theory and cases into a single decision matrix. It is useful to walk through one process and show where each layer applies.",
            "In a helpdesk: instructions for style, skills for template tasks, MCP for data, and workflow for the processing route; an agent is enabled only for non-standard cases.",
            "Comparison limitations: if evaluation criteria are set formally, the selection tree gives a false result. If available resources and team maturity are ignored, the chosen architecture does not get implemented. Also, when the process changes, the decision tree must be revised, otherwise the choice becomes outdated quickly.",
            "The best choice is not one tool, but a coordinated combination of layers for the specific process and constraints."
        ),
    )

    add_content_slide(
        prs,
        "When to Use What (Part 1)",
        [
            "One-off task -> Prompt",
            "Persistent rules needed -> Instructions",
            "Repeatable expertise -> Skills",
            "Access to systems needed -> MCPs",
            "Autonomy needed -> Agent",
            "Determinism needed -> Workflow",
        ],
        notes=educational_note(
            "Decision tree — a sequence of checks for selecting architecture. Minimum sufficient level — the simplest layer that meets the requirements.",
            "Decision tree mechanics: step 1 — task type (one-off or process), step 2 — cost of failure, step 3 — need for consistent rules, step 4 — integration requirements, step 5 — level of autonomy, step 6 — audit and SLA requirements. This algorithm makes architectural selection reproducible and defensible to the business. In the 45-minute format, this is a practical tool that participants can apply immediately.",
            "Marketing uses a prompt for one-off text; support chooses workflow + MCP for a daily regulated flow.",
            "Limitations: if evaluation criteria are set formally, the decision tree gives a false result. Not accounting for available resources and team maturity means the chosen architecture fails to get deployed. Also, when the process changes, the decision tree must be revised, otherwise the choice quickly becomes outdated.",
            "The decision tree turns architectural selection from a subjective debate into a verifiable practice."
        ),
    )

    add_content_slide(
        prs,
        "When to Use What (Part 2): Anti-patterns",
        [
            "Anti-pattern: 'agent for everything' -> expensive and unstable",
            "Anti-pattern: prompt-only in production -> no control",
            "Anti-pattern: over-privileged MCP -> security risks",
            "Anti-pattern: giant-skill -> hard to maintain",
        ],
        notes=educational_note(
            "Anti-pattern — a repeatable architectural decision that gives short-term convenience and long-term damage.",
            "Anti-pattern mechanics: first identify the symptom, then the cause, then the countermeasure. In practice: the team says 'let's make an agent for everything', then faces rising costs and unpredictability, then moves standard operations to a workflow and leaves the agent only for complex nodes. This analysis builds engineering thinking about risks in the audience.",
            "The team gave the agent broad write rights to production, which led to a chain of incorrect changes in a non-standard scenario.",
            "Limitations: anti-patterns often look like quick wins and are therefore recognized late. Without telemetry and postmortem practices, mistakes repeat. Without architecture rules at the team level, every project hits the same risks again. Additional constraint: organizational inertia means even identified anti-patterns are hard to fix without management support.",
            "Preventing anti-patterns is cheaper and safer than dealing with consequences after an incident."
        ),
    )

    add_content_slide(
        prs,
        "Case 1: Customer Support",
        [
            "Task: high volume of incoming requests",
            "Solution: Workflow + Instructions + Skills + MCP",
            "Result: routine requests are automated",
            "Complex cases escalate to an operator",
        ],
        notes=educational_note(
            "Escalation — passing a request to a human when confidence is low or criticality is high. CSAT — customer satisfaction metric.",
            "Case mechanics: an incoming request is classified, then customer context is fetched via MCP, after which skills produce a draft response within the instructions framework. The workflow then decides: auto-reply or escalation. This circuit reduces average response time, standardizes communication, and frees operators from routine. In the 45-minute presentation, this case clearly shows how the layers work together in a real process.",
            "A customer asks about a return status: the system reads data from the CRM, generates a response, and escalates disputed cases with context already gathered.",
            "Limitations: without calibrating the confidence score, the system either escalates too much or incorrectly automates complex cases. An outdated knowledge base reduces response quality even if the architecture is correct. Dependency on external APIs requires fallback procedures for service degradation. Per-stage metrics are also needed, otherwise it is hard to understand where quality is lost.",
            "The support case shows that a hybrid architecture simultaneously improves speed, quality, and risk control."
        ),
    )

    add_content_slide(
        prs,
        "Case 2: AI Code Review",
        [
            "Evolution: Prompt -> Instructions/Skills -> CI Workflow",
            "Every PR receives a fast structured review",
            "Critical findings block merge",
            "Consistent code quality growth",
        ],
        notes=educational_note(
            "CI — continuous integration. Quality gate — an automatic condition for admitting changes to the main branch.",
            "Phased implementation mechanics: 1) manual prompt checks to quickly validate value; 2) formalize criteria through instructions and skills; 3) integrate into CI workflow with an automatic quality gate. Concrete example: first the developer manually pastes a diff into chat, then the team defines standard review rules and a code-review skill, and then every PR automatically receives comments and is blocked when critical findings are found. This path reduces the risk of premature automation and provides controlled calibration. AI review does not replace engineering review but filters out routine defects and speeds up feedback.",
            "Every PR is automatically analyzed; critical security and correctness findings block merge until fixed.",
            "Limitations: without sensitivity threshold tuning, noise increases and team trust drops. Without developer feedback, rules become outdated quickly. Some architectural problems are invisible to AI without business logic context. Also need to control the cost and execution time of checks so CI does not become a bottleneck.",
            "Best results come when AI review is embedded in CI as a systemic layer and is regularly calibrated by the team."
        ),
    )

    add_content_slide(
        prs,
        "Case 3: Job Market Analytics",
        [
            "Prompt: fast, but shallow",
            "Agent + MCP: deeper and more current",
            "Workflow: scheduled regular report",
            "Phased evolution provides the best ROI",
        ],
        notes=educational_note(
            "ROI — return on investment. Normalization — bringing data from different sources to a common model.",
            "Case mechanics develops through maturity steps. First a prompt gives a quick overview for a hypothesis. Then an agent with MCP collects data from sources, cleans, aligns, and prepares an analytical summary. At the next stage a workflow establishes a regular report release, validation, and publishing. In the 45-minute story this case illustrates the core principle: complexity is added only after confirming the value of the previous step.",
            "A weekly market report includes salary trends, stack demand, and regional anomalies.",
            "Limitations: analytics quality is limited by source quality and normalization correctness. The agent can amplify noise with weak filter rules. A workflow can stably produce a wrong report if quality checks are absent. Also need to account for data update lag and seasonal effects, otherwise management decisions are based on a distorted picture.",
            "The evolutionary approach to analytics reduces risk and provides better implementation economics compared to a 'big launch' all at once."
        ),
    )

    add_content_slide(
        prs,
        "Key Takeaways",
        [
            "Main idea: these are levels, not competitors",
            "Start simple, add complexity as needed",
            "For production: control and monitoring are essential",
            "Best approach: hybrid architecture",
        ],
        notes=educational_note(
            "Observability — analyzing system state through logs, metrics, and traces. Hybrid architecture — a combination of deterministic and adaptive layers.",
            "Final decision mechanics: define the goal and cost of failure, select the minimum sufficient layer, set quality/cost metrics, then incrementally strengthen the architecture. In the 45-minute structure this slide closes the material and converts it into an action plan. It is important to bring the audience back to three questions: what are we automating, how are we controlling it, how are we measuring the effect.",
            "The team picks one pilot process for 2-4 weeks, records metrics, and after a retrospective decides on the next maturity layer.",
            "Limitations: without ownership and metrics, architecture does not scale. Without regular revision of rules/integrations, the solution becomes outdated even after a good start. Organizational constraints (security, training, change management) often matter more than technical ones. Ignoring them early slows deployment and loses business support.",
            "A sustainable AI system is a managed evolution with measurable results, not a one-time tool configuration."
        ),
    )

    add_title_slide(
        prs,
        "Local Demo",
        "How to show all levels in VS Code in 20 minutes",
        notes=educational_note(
            "Live-demo — showing working artifacts in a real IDE.",
            "This block translates theory into action: instead of example slides, you can run a live demonstration directly in VS Code. The logic is simple: the end-to-end scenario 'Customer Order Support' demonstrates the evolution from a simple prompt to instructions, skills, MCP, and an agent wrapped in a workflow. Each layer is added incrementally; the audience sees files and configs in real time.",
            "In 20 minutes you can show: creating .github/copilot-instructions.md, a skills folder with two examples, an agent, and the final workflow.",
            "Limitation: not all files can be created quickly live. MCP requires a running server, which needs preparation. If the demo breaks, the entire effect is lost. It is therefore better to prepare a backup in advance.",
            "A live demo is a powerful tool for bridging theory to practice, but requires careful preparation and rehearsal."
        ),
    )

    add_content_slide(
        prs,
        "Local Demo: Step-by-Step Scenario",
        [
            "Step 1: Basic prompt in Copilot Chat",
            "Step 2: Add copilot-instructions.md to .github",
            "Step 3: Create .github/skills folder with two skills",
            "Step 4: Create .github/agents/support-agent.agent.md",
            "Step 5: Show workflow logic as a diagram",
        ],
        notes=educational_note(
            "Step-by-step demo — a sequence of managed actions where each step builds on the previous one.",
            "The demo flow: first open VS Code in the project folder and type a simple prompt in Copilot Chat. Then create .github/copilot-instructions.md with persistent rules. Next quickly create two skill folders: one for customer-reply, one for policy-check. After that create an agent that uses both skills. Finally show on a diagram or in code how everything ties together in a workflow. Each step takes 2-3 minutes, total 15-20 minutes.",
            "The audience sees files, configs, and how they connect into a unified system.",
            "Risk: if the internet is slow or extensions fail to load, the demo can stall. Also need to prepare a clean folder in advance. Without rehearsal it is easy to get lost in details and overrun the time.",
            "A successful live demo is remembered better than 10 text slides."
        ),
    )

    add_title_slide(
        prs,
        "Thank You!",
        "Questions and Discussion",
        notes=educational_note(
            "Implementation roadmap — a concrete plan for moving from a pilot to an operational circuit.",
            "Final mechanics: pick one real case from the audience, quickly run the decision tree, identify the target layer, risks, metrics, and the nearest step. This completion format turns the presentation into a practical tool. In a 45-minute session this block is important for consolidation: listeners should leave with a clear action, not just theory.",
            "Using the customer request example, the team agrees on an MVP architecture, success metrics, and the timeline for the first iteration.",
            "Limitations: without an assigned owner and a date, the next step often does not happen. Without a realistic assessment of available access and data, the roadmap stays theoretical. There is also a risk of vague discussion without prioritization, causing the team not to start the pilot.",
            "The training outcome should be practical: a clear next step, ownership, and success criteria."
        ),
    )

    prs.save(str(OUT))
    apply_gallery_theme(OUT)
    print(f"Saved: {OUT}")


if __name__ == "__main__":
    build()
