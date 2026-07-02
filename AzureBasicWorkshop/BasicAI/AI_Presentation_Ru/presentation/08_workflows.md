# 🔄 Workflows — Детерминированные AI-пайплайны

---

## 1. Заголовок и краткое определение

**Workflow (рабочий процесс)** — это заранее определённая последовательность шагов, где AI участвует в одном или нескольких узлах, но общая логика и порядок выполнения управляются кодом, а не самим AI. Workflow обеспечивает предсказуемость и надёжность там, где гибкость агентов неприемлема.

---

## 2. Подробное объяснение

### Простыми словами
Представь рецепт приготовления блюда. Шаги строго определены: сначала режешь лук, потом обжариваешь, потом добавляешь мясо. Порядок нельзя изменить — иначе блюдо испортится. AI может участвовать на отдельных этапах (например, предлагать специи), но общий рецепт фиксирован.

Workflow = зафиксированный рецепт, где AI — один из поваров, а не шеф-повар.

### Технически
В Workflow:
- **Граф выполнения задаётся кодом** (не AI)
- **AI участвует в узлах** как «умная функция»
- **Переходы** между шагами определены заранее (иногда с условиями)
- **Детерминизм**: одинаковый ввод → (почти) одинаковый результат
- **Оркестраторы**: LangGraph, Azure Logic Apps, Apache Airflow, n8n, Temporal

```
┌──────────────────────────────────────────────────────────────┐
│                   WORKFLOW EXAMPLE                           │
│                    (Обработка заявки)                        │
│                                                              │
│  [START]                                                     │
│    │                                                         │
│    ▼                                                         │
│  [Получить заявку]  ──── код читает из очереди              │
│    │                                                         │
│    ▼                                                         │
│  [Классифицировать] ──── AI определяет тип заявки           │
│    │                                                         │
│    ├─── "Техническая" ──► [Tech Queue]                       │
│    │                                                         │
│    ├─── "Финансовая"  ──► [Finance Queue]                    │
│    │                                                         │
│    └─── "Другое"      ──► [AI отвечает сам] ──────────────┐  │
│                                                            │  │
│  [Tech Queue]                                             │  │
│    │                                                       │  │
│    ▼                                                       │  │
│  [AI генерирует ответ] ──── AI + Knowledge Base           │  │
│    │                                                       │  │
│    ▼                                                       │  │
│  [Проверка качества] ──── автоматическая валидация        │  │
│    │                                                       │  │
│    ├─── OK ──► [Отправить клиенту] ◄──────────────────────┘  │
│    │                                                          │
│    └─── FAIL ──► [Human Review]                              │
│                                                              │
│  [END]                                                       │
└──────────────────────────────────────────────────────────────┘
```

### Ключевые отличия от простой последовательности вызовов:
- Поддержка **ветвления** (if/else)
- Поддержка **параллельного выполнения** (fan-out/fan-in)
- **State management** — состояние передаётся между шагами
- **Error handling** — обработка ошибок на уровне пайплайна
- **Retries и timeouts** — встроены в оркестратор

---

## 3. Аналогия из реальной жизни

**Workflow = Производственный конвейер**

На заводе Toyota:
1. Сварка кузова — автоматическая
2. Нанесение краски — роботизированная
3. Установка двигателя — частично ручная
4. Контроль качества — автоматизированный тест
5. Финальная проверка — человек

Порядок строго определён. Никто не решает «может пропустим покраску?». Если на шаге 4 обнаружен дефект — машина идёт на переработку, а не в следующий шаг. AI на шаге 3 помогает точнее установить двигатель, но не решает, нужен ли двигатель вообще.

**Сравнение с агентом:**
- Агент = Изобретательный ремесленник: «Клиент хочет стул. Посмотрю, что есть в наличии, и придумаю.»
- Workflow = Фабрика: «Стул → план → нарезка → шлифовка → сборка → покраска → QC → отправка»

---

## 4. Как это работает — Механика

### Типы шагов в Workflow:

```
┌─────────────────────────────────────────────────────────┐
│  ТИПЫ УЗЛОВ В WORKFLOW                                  │
│                                                         │
│  [AI Node]     — вызов LLM для генерации/анализа        │
│  [Code Node]   — выполнение кода (Python, JS)           │
│  [MCP Node]    — вызов внешнего инструмента             │
│  [Branch Node] — if/else ветвление                      │
│  [Fan-out]     — параллельный запуск нескольких путей   │
│  [Fan-in]      — сбор результатов параллельных путей    │
│  [Human Node]  — ожидание ответа от человека            │
│  [Timer Node]  — задержка или расписание                │
└─────────────────────────────────────────────────────────┘
```

### Сравнение подходов Agents vs Workflows:

```
ЗАДАЧА: "Обработать 1000 документов и создать отчёт"

──── AGENT подход ────────────────────────────────────────────
Агент получает задачу → сам решает как → итеративно обрабатывает
Проблемы:
  ✗ Может обработать документы в другом порядке
  ✗ Нет гарантии что все 1000 обработаны
  ✗ Если упал на #500 — нужно начинать заново
  ✗ Сложно мониторить прогресс

──── WORKFLOW подход ─────────────────────────────────────────
Оркестратор: 
  for doc in documents:  # строго по порядку
    step1: validate(doc)
    step2: extract_data(doc)    # AI
    step3: classify(doc)        # AI
    step4: save_to_db(result)
    step5: update_progress()
  step6: generate_report()      # AI
  
Преимущества:
  ✓ Идемпотентность — можно запустить повторно с места остановки
  ✓ Чёткий прогресс — 347/1000 обработано
  ✓ Обработка ошибок — если шаг 2 упал, остальные не затронуты
  ✓ Мониторинг — каждый шаг логируется
```

---

## 5. Примеры кода и использования

### Пример 1: LangGraph — граф с AI узлами
```python
from langgraph.graph import StateGraph, END
from langchain_anthropic import ChatAnthropic
from typing import TypedDict, List

# Состояние передаётся между узлами
class SupportTicketState(TypedDict):
    ticket_text: str
    category: str
    draft_response: str
    quality_score: float
    final_response: str

llm = ChatAnthropic(model="claude-opus-4-5")

# Узел 1: Классификация (AI)
def classify_ticket(state: SupportTicketState) -> dict:
    response = llm.invoke(
        f"Классифицируй обращение: {state['ticket_text']}\n"
        "Варианты: technical, billing, general\n"
        "Ответь одним словом."
    )
    return {"category": response.content.strip()}

# Узел 2: Генерация ответа (AI)
def generate_response(state: SupportTicketState) -> dict:
    response = llm.invoke(
        f"Категория: {state['category']}\n"
        f"Обращение: {state['ticket_text']}\n"
        "Напиши профессиональный ответ клиенту."
    )
    return {"draft_response": response.content}

# Узел 3: Проверка качества (код)
def check_quality(state: SupportTicketState) -> dict:
    response = state["draft_response"]
    # Простая проверка: длина, наличие приветствия
    score = 0.0
    if len(response) > 50: score += 0.4
    if "Здравствуйте" in response or "Добрый" in response: score += 0.3
    if "С уважением" in response: score += 0.3
    return {"quality_score": score}

# Узел 4: Финализация или эскалация
def finalize_or_escalate(state: SupportTicketState) -> str:
    """Ветвление — определяет следующий узел"""
    if state["quality_score"] >= 0.7:
        return "send_response"
    else:
        return "escalate_to_human"

def send_response(state: SupportTicketState) -> dict:
    send_email(state["draft_response"])
    return {"final_response": state["draft_response"]}

def escalate_to_human(state: SupportTicketState) -> dict:
    notify_team(state["ticket_text"], state["draft_response"])
    return {"final_response": "escalated"}

# Строим граф
workflow = StateGraph(SupportTicketState)

workflow.add_node("classify", classify_ticket)
workflow.add_node("generate", generate_response)
workflow.add_node("check", check_quality)
workflow.add_node("send_response", send_response)
workflow.add_node("escalate_to_human", escalate_to_human)

workflow.set_entry_point("classify")
workflow.add_edge("classify", "generate")
workflow.add_edge("generate", "check")
workflow.add_conditional_edges("check", finalize_or_escalate)
workflow.add_edge("send_response", END)
workflow.add_edge("escalate_to_human", END)

app = workflow.compile()

# Запуск
result = app.invoke({
    "ticket_text": "У меня не работает оплата картой Visa"
})
```

### Пример 2: n8n — No-code AI Workflow (JSON конфиг)
```json
{
  "name": "Document Processing Pipeline",
  "nodes": [
    {
      "id": "1",
      "name": "Trigger: New File",
      "type": "n8n-nodes-base.webhook",
      "parameters": {"path": "/new-document"}
    },
    {
      "id": "2", 
      "name": "Extract Text",
      "type": "n8n-nodes-base.extractFromFile",
      "parameters": {"operation": "extractFromPDF"}
    },
    {
      "id": "3",
      "name": "AI: Classify Document",
      "type": "@n8n/n8n-nodes-langchain.openAi",
      "parameters": {
        "operation": "text",
        "prompt": "Классифицируй документ: {{ $json.text }}\nТипы: invoice, contract, report"
      }
    },
    {
      "id": "4",
      "name": "Route by Type",
      "type": "n8n-nodes-base.switch",
      "parameters": {
        "rules": {
          "invoice": "save_to_accounting",
          "contract": "save_to_legal",
          "report": "save_to_reports"
        }
      }
    }
  ]
}
```

### Пример 3: Azure Logic Apps для AI Workflow
```json
{
  "definition": {
    "triggers": {
      "When_email_arrives": {
        "type": "ApiConnection",
        "inputs": {"host": {"connection": {"name": "office365"}}}
      }
    },
    "actions": {
      "Analyze_sentiment": {
        "type": "ApiConnection",
        "inputs": {
          "host": {"connection": {"name": "cognitiveservicestextanalytics"}},
          "method": "post",
          "path": "/text/analytics/v3.0/sentiment",
          "body": {"documents": [{"text": "@triggerBody()?['Body']"}]}
        }
      },
      "Route_by_sentiment": {
        "type": "If",
        "expression": {
          "equals": ["@body('Analyze_sentiment')?['sentiment']", "negative"]
        },
        "actions": {
          "Create_urgent_ticket": {"...": "..."},
          "Notify_manager": {"...": "..."}
        },
        "else": {
          "actions": {
            "Auto_respond": {"...": "..."}
          }
        }
      }
    }
  }
}
```

### Пример 4: Гибридный подход — Workflow с Агентом внутри
```python
# Workflow с агентом как один из шагов
class HybridPipeline:
    
    def run(self, document: str) -> dict:
        # Шаг 1: Детерминированный — парсинг (код)
        structured_data = self.parse_document(document)
        
        # Шаг 2: AI-агент — исследование (агент выбирает стратегию)
        if structured_data["type"] == "complex_analysis":
            agent_result = self.research_agent.run(
                goal=f"Исследуй все аспекты: {structured_data}",
                max_steps=20
            )
        else:
            agent_result = self.simple_llm_call(structured_data)
        
        # Шаг 3: Детерминированный — сохранение (код)
        report_id = self.save_report(agent_result)
        
        # Шаг 4: Детерминированный — уведомление (код)
        self.notify_stakeholders(report_id)
        
        return {"report_id": report_id, "status": "completed"}
```

---

## 6. Плюсы

1. **Надёжность и предсказуемость** — производственный код, а не «может агент придумает». Можно прогнать 10 000 документов с гарантией обработки каждого.

2. **Мониторинг и обсервабилити** — каждый шаг логируется, есть метрики, алерты. Легко ответить: «Где затык? Сколько обработано?»

3. **Восстановление после сбоев** — если шаг 5 из 10 упал, можно перезапустить с шага 5, а не с начала (checkpoint-based restart).

4. **Тестируемость** — каждый узел тестируется изолированно. Unit-тесты для AI-workflow — реальная вещь.

5. **Контроль затрат** — заранее знаешь, сколько LLM-вызовов делает каждый запуск. Нет неожиданных расходов.

6. **Compliance** — для регулируемых отраслей (банки, медицина) аудиторский след workflow — требование, не опция.

---

## 7. Минусы / Ограничения

1. **Жёсткость** — если сценарий не предусмотрен в графе → workflow не знает, что делать. Нужно добавлять новые ветки вручную.

2. **Затраты на создание** — построить хороший workflow дольше, чем написать промпт или создать агента. Это инвестиция в долгосрочную надёжность.

3. **Maintenance overhead** — при изменении бизнес-процесса нужно обновлять граф. Если process owners не вовлечены — workflow устаревает.

4. **Избыточность для простых задач** — если задача простая и не повторяется, workflow — оверкилл. Используй промпт.

5. **AI-компоненты всё равно недетерминированы** — workflow детерминирован на уровне потока, но AI-узлы внутри — стохастические. Нужны собственные меры стабилизации (temperature=0, few-shot примеры).

---

## 8. Когда использовать / Когда НЕ использовать

### ✅ Используй Workflows когда:
- Бизнес-процесс с чёткими шагами и правилами
- Нужна надёжность и предсказуемость на продакшене
- Большой объём повторяющихся задач (batch processing)
- Регулируемая отрасль требует аудиторского следа
- Несколько команд/систем участвуют в процессе
- Критична возможность восстановления после сбоев

### ❌ НЕ используй Workflows когда:
- Задача исследовательская — нельзя заранее описать шаги (используй Agent)
- Разовая задача без повторений (используй Prompt или Agent)
- Нужна максимальная адаптивность
- Команда не имеет времени на поддержку

---

## 9. Сравнение с соседними концептами

### Workflows vs Agents — Ключевое сравнение

| Критерий | Agents | Workflows |
|----------|--------|-----------|
| Контроль потока | AI (динамически) | Код (статично) |
| Предсказуемость | Низкая | Высокая |
| Гибкость | Высокая | Низкая |
| Надёжность | Средняя | Высокая |
| Стоимость LLM | Высокая | Умеренная |
| Тестируемость | Сложная | Простая |
| Восстановление | Сложное | Встроено |
| Мониторинг | Сложный | Встроен |
| Идеально для | Исследование, нестандартное | Бизнес-процессы, batch |

### Гибридная модель: лучшее из двух миров

```
WORKFLOW (надёжная оболочка)
├── Шаг 1: Получить входные данные   [детерминированный код]
├── Шаг 2: Обработать              [AGENT — адаптивный]
│   ├── Агент может исследовать
│   ├── Вызывать инструменты
│   └── Принимать решения
├── Шаг 3: Валидировать результат   [детерминированный код]
└── Шаг 4: Сохранить и уведомить   [детерминированный код]
```

---

## 10. Заметки для спикера

- **Аналогия конвейера** — абсолютно понятна для любой аудитории. Toyota Lean — хорошая отсылка
- **Живая демонстрация n8n** или **LangGraph** визуального графа — очень наглядно
- **Agents vs Workflows** — финальная битва этого раздела. Повтори: «Не «лучше/хуже», а «для разных задач»»
- **Compliance аспект** — для банковской/страховой аудитории это ключевой аргумент в пользу workflow
- **Гибридная модель** — это архитектурный паттерн, который работает в реальных продуктах. Приведи пример из своей практики
- Время на этот слайд: **7–8 минут**

---

## 11. Возможные вопросы аудитории

**В: В чём разница между Workflow и просто последовательными API-вызовами в коде?**
> О: Оркестраторы workflow дают: (1) визуальное представление и документацию процесса, (2) built-in retry и error handling, (3) checkpoint и resume при сбоях, (4) мониторинг и метрики из коробки, (5) версионирование процессов. Просто вызовы API ничего из этого не дают — нужно писать самому.

**В: Как выбрать инструмент для workflow — Airflow, n8n, LangGraph, Logic Apps?**
> О: Ориентир: (1) **LangGraph** — если workflow сложный, много AI-узлов, нужна гибкость Python, (2) **n8n** — если нужен no-code/low-code, интеграция с 400+ сервисами, (3) **Azure Logic Apps** — если уже в Azure экосистеме, нужен managed сервис, (4) **Airflow** — если batch processing больших данных, есть команда data engineers.

**В: Можно ли изменить Workflow на лету, без перезапуска?**
> О: Зависит от инструмента. LangGraph поддерживает динамическое изменение графа. Большинство oркестраторов — нет: нужно деплоить новую версию. Это trade-off надёжности: изменяемость на лету опасна в продакшене.

**В: Как мониторить AI-узлы внутри Workflow?**
> О: (1) Логировать вход/выход каждого AI-узла, (2) Отслеживать latency и cost каждого LLM-вызова, (3) Использовать специализированные инструменты — LangSmith, Azure AI Foundry Traces, (4) Настраивать алерты на аномальные ответы (слишком длинные, слишком короткие, ошибки).

---

*← [07_agents.md](07_agents.md) | [09_comparison.md](09_comparison.md) →*
