using Microsoft.Extensions.Logging;

namespace WebAppGateway.Messaging;

public sealed class RetryPolicyExecutor : IRetryPolicyExecutor
{
    private readonly ILogger<RetryPolicyExecutor> _logger;

    public RetryPolicyExecutor(ILogger<RetryPolicyExecutor> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        int attempts,
        int baseDelayMs,
        CancellationToken cancellationToken)
    {
        if (attempts < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(attempts), "Retry attempts must be greater than 0.");
        }

        var jitter = new Random();

        for (var currentAttempt = 1; currentAttempt <= attempts; currentAttempt++)
        {
            try
            {
                await action(cancellationToken);
                return;
            }
            catch (Exception ex) when (currentAttempt < attempts)
            {
                var delayMs = (int)Math.Pow(2, currentAttempt - 1) * baseDelayMs + jitter.Next(50, 200);
                _logger.LogWarning(
                    ex,
                    "Transient failure on attempt {Attempt}/{TotalAttempts}. Retrying in {DelayMs} ms.",
                    currentAttempt,
                    attempts,
                    delayMs);

                await Task.Delay(delayMs, cancellationToken);
            }
        }

        await action(cancellationToken);
    }
}