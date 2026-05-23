using System.Text;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionProcessor.Functions;

public sealed class EventHubFunction
{
    private readonly ILogger<EventHubFunction> _logger;

    public EventHubFunction(ILogger<EventHubFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessEventHubEvents")]
    public void Run(
        [EventHubTrigger(
            "%EventHubName%",
            Connection = "EventHubConnection",
            ConsumerGroup = "%EventHubConsumerGroup%")]
        EventData[] events)
    {
        foreach (var eventData in events)
        {
            var payload = Encoding.UTF8.GetString(eventData.EventBody.ToArray());
            _logger.LogInformation(
                "Event Hub event processed. PartitionKey={PartitionKey}, PayloadLength={Length}",
                eventData.PartitionKey,
                payload.Length);
        }
    }
}