using BuildHub.CommandBuilder.Models.Execution;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.Models.Unused.Steps;

/// <summary>
/// File operation step configuration.
/// </summary>
public sealed class FileStep : ExecutionStep
{
	/// <summary>
	/// File system operation to perform.
	/// </summary>
	public FileOperation Operation { get; init; }

	/// <summary>
	/// Path associated with the file operation.
	/// </summary>
	public string Path { get; init; } = string.Empty;
}
