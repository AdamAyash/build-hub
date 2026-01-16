namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context for TFS (Team Foundation Server) command builder.
/// Contains information for versioning, workspace, and changeset management.
/// </summary>
public class TfsContext
{
	/// <summary>
	/// Gets or sets the workspace path to retrieve.
	/// </summary>
	public string WorkspacePath { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the changeset number to retrieve.
	/// If null or empty, the latest version is used.
	/// </summary>
	public string? Changeset { get; set; }

	/// <summary>
	/// Gets or sets whether the get operation is recursive.
	/// </summary>
	public bool Recursive { get; set; } = true;

	/// <summary>
	/// Gets or sets whether files are forcibly overwritten.
	/// </summary>
	public bool Force { get; set; } = true;
}
