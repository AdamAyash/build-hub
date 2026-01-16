namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context for Visual Studio command builder.
/// Includes paths and outputs specific to Visual Studio builds.
/// </summary>
public class VisualStudioContext : MsBuildBasedContext
{
	/// <summary>
	/// Path to the output file produced by the build.
	/// </summary>
	public string? OutputFile { get; set; }
}
