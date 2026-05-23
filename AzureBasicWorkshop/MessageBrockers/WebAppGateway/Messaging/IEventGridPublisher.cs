using Contracts;

namespace WebAppGateway.Messaging;

public interface IEventGridPublisher
{
    Task PublishAsync(MessageEnvelope envelope, CancellationToken cancellationToken);
}