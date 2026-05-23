using Contracts;

namespace WebAppGateway.Messaging;

public interface IServiceBusPublisher
{
    Task SendToQueueAsync(MessageEnvelope envelope, CancellationToken cancellationToken);

    Task SendToTopicAsync(MessageEnvelope envelope, CancellationToken cancellationToken);
}