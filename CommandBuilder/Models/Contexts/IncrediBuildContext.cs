using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context for IncrediBuild command builder.
/// Contains properties specific to distributed IncrediBuild execution.
/// </summary>
public class IncrediBuildContext : MsBuildBasedContext
{
	/// <summary>
	/// Path to the output file produced by the build.
	/// </summary>
	public string OutputFile { get; set; } = default!;

	/// <summary>
	/// Version produced by this build definition.
	/// </summary>
	public BuildVersion Version { get; set; } = default!;

	/// <summary>
	/// Visual Studio version used for the build, when applicable.
	/// </summary>
	public VisualStudioVersions VisualStudioVersion { get; set; } = VisualStudioVersions.VS2017;

	/// <summary>
	/// Indicates whether MSBuild is used explicitly for the build.
	/// </summary>
	public bool UseMsBuild { get; set; } = false;
}
