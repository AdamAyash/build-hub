namespace BuildHub.CommandBuilder.Models.Execution.Enums;

/// <summary>
/// Step types supported in pipelines.
/// </summary>
public enum StepType
{
	Build,
	Script,
	File,
	Command,
	Source
}
