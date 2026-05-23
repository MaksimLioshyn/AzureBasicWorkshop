using System.Text.Json;
using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionProcessor.Functions;

public sealed class EventGridFunction
{
    private readonly ILogger<EventGridFunction> _logger;

    public EventGridFunction(ILogger<EventGridFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessEventGridEvent")]
    public void Run([EventGridTrigger] CloudEvent cloudEvent)
    {
        var dataJson = JsonSerializer.Serialize(cloudEvent.Data);

        _logger.LogInformation(
            "Event Grid event processed. Type={Type}, Subject={Subject}, Data={Data}",
            cloudEvent.Type,
            cloudEvent.Subject,
            dataJson);
    }
}