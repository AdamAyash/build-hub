using BuildHub.CommandBuilder.Models.Execution;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.Models.Unused.Steps;

/// <summary>
/// Defines how source code is fetched before building.
/// </summary>
public sealed class SourceStep : ExecutionStep
{
	/// <summary>
	/// Source control system to use.
	/// </summary>
	public SourceProvider Provider { get; init; }

	/// <summary>
	/// Repository URL or TFS collection path.
	/// </summary>
	public string Repository { get; init; } = string.Empty;

	/// <summary>
	/// Branch name (Git) or workspace path (TFS).
	/// </summary>
	public string Branch { get; init; } = "main";

	/// <summary>
	/// Optional commit, tag, or changeset.
	/// If null, latest is retrieved.
	/// </summary>
	public string? Version { get; init; }

	/// <summary>
	/// Relative checkout directory.
	/// </summary>
	public string TargetDirectory { get; init; } = string.Empty;

	/// <summary>
	/// Whether to clean the workspace before fetching.
	/// </summary>
	public bool Clean { get; init; } = false;
}

