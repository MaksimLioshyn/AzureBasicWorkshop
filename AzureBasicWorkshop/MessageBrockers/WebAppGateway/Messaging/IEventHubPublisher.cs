using Contracts;

namespace WebAppGateway.Messaging;

public interface IEventHubPublisher
{
    Task PublishAsync(MessageEnvelope envelope, CancellationToken cancellationToken);
}