using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace AzureOpenAIConsoleDemoWorkflow;

/// <summary>
/// Provides practical Azure OpenAI demo scenarios for live presentation.
/// </summary>
public sealed class AzureOpenAIPracticeWorkflow
{
    private readonly AzureOpenAiSettings _settings;

    /// <summary>
    /// Initializes the workflow and loads Azure OpenAI settings from appsettings.json.
    /// </summary>
    public AzureOpenAIPracticeWorkflow()
    {
        _settings = LoadAzureOpenAiSettings();
    }

    /// <summary>
    /// Runs a basic prompt scenario and returns the model answer.
    /// </summary>
    /// <returns>A short response text suitable for a live demo.</returns>
    public async Task<string> RunBasicPromptAsync()
    {
        // If Azure OpenAI settings are configured, this method runs a real model call.
        // If not configured (or unavailable), fallback text keeps the presentation stable.
        var prompt = "Explain Azure OpenAI in one concise sentence for developers.";
        var text = await TryCompleteChatAsync(
            systemPrompt: "You are a concise technical assistant.",
            userPrompt: prompt);

        return text ?? "Azure OpenAI can generate concise answers from a natural language prompt.";
    }

    /// <summary>
    /// Runs a structured-output scenario and returns parsed JSON-like data.
    /// </summary>
    /// <param name="ticketText">A support ticket text used as model input.</param>
    /// <returns>A normalized support ticket analysis result.</returns>
    public async Task<SupportTicketAnalysis> RunStructuredOutputAsync(string ticketText)
    {
        // This prompt asks the model to return strict JSON so we can map to a typed C# object.
        var jsonPrompt =
            "Return ONLY JSON with fields: category, severity, summary. " +
            "Severity must be one of: Low, Medium, High. " +
            $"Ticket: {ticketText}";

        var modelText = await TryCompleteChatAsync(
            systemPrompt: "You extract structured data for backend processing.",
            userPrompt: jsonPrompt);

        if (!string.IsNullOrWhiteSpace(modelText))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<SupportTicketAnalysis>(modelText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed is not null)
                {
                    return parsed;
                }
            }
            catch (JsonException)
            {
                // Keep presentation resilient: if model returns non-JSON text, fallback to deterministic output.
            }
        }

        return new SupportTicketAnalysis
        {
            Category = "Authentication",
            Severity = "High",
            Summary = $"Issue detected from ticket: {ticketText}"
        };
    }

    /// <summary>
    /// Runs a practical business scenario and returns prioritized action items.
    /// </summary>
    /// <param name="tickets">A list of raw support tickets.</param>
    /// <returns>Action items that can be shown as a team-ready result.</returns>
    public async Task<BusinessCaseResult> RunBusinessCaseAsync(IReadOnlyList<string> tickets)
    {
        // Real model path: summarize all tickets into a short operations action list.
        var mergedTickets = string.Join(Environment.NewLine, tickets.Select((t, i) => $"{i + 1}. {t}"));
        var prompt =
            "Create up to 5 short action items for engineering operations. " +
            "Return each action item on a new line." + Environment.NewLine +
            mergedTickets;

        var modelText = await TryCompleteChatAsync(
            systemPrompt: "You produce practical and prioritized engineering action items.",
            userPrompt: prompt);

        if (!string.IsNullOrWhiteSpace(modelText))
        {
            var modelItems = modelText
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(line => line.TrimStart('-', '*', ' '))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (modelItems.Count > 0)
            {
                return new BusinessCaseResult
                {
                    ActionItems = modelItems
                };
            }
        }

        // Fallback path: deterministic output for offline demos.
        var actionItems = tickets
            .Select((ticket, index) => $"P{index + 1}: Investigate and resolve - {ticket}")
            .ToList();

        return new BusinessCaseResult
        {
            ActionItems = actionItems
        };
    }

    private async Task<string?> TryCompleteChatAsync(string systemPrompt, string userPrompt)
    {
        if (string.IsNullOrWhiteSpace(_settings.Endpoint) || string.IsNullOrWhiteSpace(_settings.DeploymentName))
        {
            return null;
        }

        try
        {
            var client = CreateChatClient(_settings);
            var options = new ChatCompletionOptions
            {
                Temperature = _settings.Temperature
            };

            var completion = await client.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            ],
            options);

            return completion.Value.Content.FirstOrDefault()?.Text;
        }
        catch (Exception)
        {
            // Any transient/network/auth issue should not break the presentation flow.
            return null;
        }
    }

    private static ChatClient CreateChatClient(AzureOpenAiSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            var azureClient = new AzureOpenAIClient(new Uri(settings.Endpoint), new AzureKeyCredential(settings.ApiKey));
            return azureClient.GetChatClient(settings.DeploymentName);
        }

        // Managed identity / developer credential path for secure cloud-native auth.
        var entraClient = new AzureOpenAIClient(new Uri(settings.Endpoint), new DefaultAzureCredential());
        return entraClient.GetChatClient(settings.DeploymentName);
    }

    private static AzureOpenAiSettings LoadAzureOpenAiSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        return configuration.GetSection("AzureOpenAI").Get<AzureOpenAiSettings>() ?? AzureOpenAiSettings.Empty;
    }

    private sealed class AzureOpenAiSettings
    {
        public static AzureOpenAiSettings Empty => new();

        public string Endpoint { get; init; } = string.Empty;

        public string DeploymentName { get; init; } = string.Empty;

        public string? ApiKey { get; init; }

        public float Temperature { get; init; } = 1.0f;
    }
}

/// <summary>
/// Represents a structured result extracted from support text.
/// </summary>
public sealed class SupportTicketAnalysis
{
    /// <summary>
    /// Gets or sets the issue category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a short issue summary.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Represents business-ready output from analyzed ticket inputs.
/// </summary>
public sealed class BusinessCaseResult
{
    /// <summary>
    /// Gets or sets prioritized action items.
    /// </summary>
    public IReadOnlyList<string> ActionItems { get; set; } = [];
}
