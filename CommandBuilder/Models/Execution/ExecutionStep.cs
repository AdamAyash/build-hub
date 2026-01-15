namespace BuildHub.CommandBuilder.Models.Execution;

/// <summary>
/// Represents a single step in a execution plan.
/// </summary>
public class ExecutionStep
{
    /// <summary>
    /// Unique identifier of the execution step.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Index of execution in the execution. Lower numbers execute first.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Human-readable name of the step.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Identifies execution plugin (msbuild, git, script...)</summary>
    public string StepType { get; init; } = string.Empty;

    /// <summary>Commands executed sequentially</summary>
    public IReadOnlyList<ExecutionCommand> Commands { get; init; } = Array.Empty<ExecutionCommand>();

    /// <summary>Dependencies (DAG-ready)</summary>
    public IReadOnlyList<Guid> DependsOn { get; init; } = Array.Empty<Guid>();

    /// <summary>Typed plugin-specific data</summary>
    public object? Metadata { get; init; }

    public RetryPolicy? RetryPolicy { get; init; }
    public TimeoutPolicy? TimeoutPolicy { get; init; }

    public IReadOnlyDictionary<string, string>? EnvironmentVariables { get; init; }
}