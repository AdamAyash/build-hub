namespace BuildHub.CommandBuilder.Models.Execution;

/// <summary>
/// Represents a reusable execution plan definition containing ordered steps.
/// </summary>
public class ExecutionPlan
{
	/// <summary>
	/// Unique identifier for the execution plan.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Human-readable name of the execution plan.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Version produced by this build definition.
	/// </summary>
	public BuildVersion Version { get; set; } = new();

	/// <summary>
	/// Ordered list of steps that make up the execution plan.
	/// </summary>
	public List<ExecutionStep> Steps { get; set; } = new List<ExecutionStep>();
}
