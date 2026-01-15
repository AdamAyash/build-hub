using BuildHub.CommandBuilder.Models.Execution;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.Models.Unused.Steps;

/// <summary>
/// Script execution step configuration.
/// </summary>
public sealed class ScriptStep : ExecutionStep
{
    /// <summary>
    /// Shell environment used to execute the script.
    /// </summary>
    public ScriptShell Shell { get; init; }

    /// <summary>
    /// Path to the script file to be executed.
    /// </summary>
    public string ScriptPath { get; init; } = string.Empty;
}
