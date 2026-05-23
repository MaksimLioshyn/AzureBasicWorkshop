namespace WebAppGateway.Options;

public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";

    public string ServiceBusFullyQualifiedNamespace { get; set; } = string.Empty;

    public string ServiceBusQueueName { get; set; } = "demo-queue";

    public string ServiceBusTopicName { get; set; } = "demo-topic";

    public string EventHubFullyQualifiedNamespace { get; set; } = string.Empty;

    public string EventHubName { get; set; } = "demo-hub";

    public string EventGridTopicEndpoint { get; set; } = string.Empty;

    public int RetryAttempts { get; set; } = 3;

    public int RetryBaseDelayMs { get; set; } = 400;
}