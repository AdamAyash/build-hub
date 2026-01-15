namespace BuildHub.CommandBuilder.Models;

public sealed class TimeoutPolicy
{
    /// <summary>Maximum allowed execution time per attempt.</summary>
    public TimeSpan Timeout { get; init; }

    /// <summary>
    /// Whether the process should be forcefully killed
    /// if timeout is exceeded.
    /// </summary>
    public bool KillProcessTree { get; init; } = true;

    public override string ToString()
    {
        return $"Timeout policy: Timeout: {Timeout} kill process: {KillProcessTree}";
    }
}
