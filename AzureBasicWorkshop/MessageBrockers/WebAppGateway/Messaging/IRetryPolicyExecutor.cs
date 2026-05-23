namespace WebAppGateway.Messaging;

public interface IRetryPolicyExecutor
{
    Task ExecuteAsync(Func<CancellationToken, Task> action, int attempts, int baseDelayMs, CancellationToken cancellationToken);
}