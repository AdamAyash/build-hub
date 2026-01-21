namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context specific to MSBuild command builder.
/// Includes version information produced by the build.
/// </summary>
public sealed class MsBuildContext : MsBuildBasedContextBase
{
	/// <summary>
	/// Version produced by this build definition.
	/// </summary>
	public BuildVersion Version { get; set; } = new();
}
