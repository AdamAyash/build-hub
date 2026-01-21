using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models.Enums;

namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Represents the base context for MSBuild-based builders, containing common build settings.
/// </summary>
public class MsBuildBasedContextBase : ICommandBuilderContext
{
	/// <summary>
	/// Type of build operation to perform (e.g. Build or Rebuild).
	/// </summary>
	public BuildType BuildType { get; set; } = BuildType.Build;

	/// <summary>
	/// Build flavor used during the build process (e.g. Release or Debug).
	/// </summary>
	public BuildFlavor Flavor { get; set; } = BuildFlavor.Release;

	/// <summary>
	/// Target platform for the build (e.g. x64, AnyCPU).
	/// </summary>
	public string Platform { get; set; } = string.Empty;

	/// <summary>
	/// Build target or task name (e.g. an NPM script name or a Gradle task).
	/// </summary>
	public string Target { get; set; } = string.Empty;
}
