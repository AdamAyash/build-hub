namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context specific to MSBuild command builder.
/// Includes version information produced by the build.
/// </summary>
public class MsBuildContext : MsBuildBasedContext
{
	/// <summary>
	/// Version produced by this build definition.
	/// </summary>
	public BuildVersion Version { get; set; } = new();
}
