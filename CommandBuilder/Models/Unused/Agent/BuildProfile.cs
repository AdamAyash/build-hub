using BuildHub.CommandBuilder.Models.Enums;

namespace BuildHub.CommandBuilder.Models.Unused.Agent;

/// <summary>
/// Represents a build profile for an agent, specifying flavor-specific configuration.
/// </summary>
public class BuildProfile
{
	/// <summary>
	/// The build flavor this profile applies to (Debug, Release, etc.).
	/// </summary>
	public BuildFlavor Flavor { get; set; }

	/// <summary>
	/// Base path on the agent where the build steps are executed.
	/// </summary>
	public string BasePath { get; set; } = string.Empty;

	/// <summary>
	/// Optional environment variables specific to this profile.
	/// </summary>
	public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
}
