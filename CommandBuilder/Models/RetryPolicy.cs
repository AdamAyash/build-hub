using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.Models;

public sealed class RetryPolicy
{
    /// <summary>Maximum number of attempts including the first one.</summary>
    public int MaxAttempts { get; init; } = 1;

    /// <summary>Delay between retry attempts.</summary>
    public TimeSpan Delay { get; init; } = TimeSpan.Zero;

    /// <summary>
    /// Whether retry should occur for non-zero exit codes only,
    /// or also for timeouts / execution exceptions.
    /// </summary>
    public RetryOn RetryOn { get; init; } = RetryOn.FailuresOnly;

    public override string ToString()
    {
        return $"Retry policy: Max retries: {MaxAttempts} timeout: {Delay} retry on: {RetryOn.ToString()}";
    }
}
