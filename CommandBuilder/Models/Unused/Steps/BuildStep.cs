using BuildHub.CommandBuilder.Models.Enums;
using BuildHub.CommandBuilder.Models.Execution;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.Models.Unused.Steps;

/// <summary>
/// Build step configuration.
/// </summary>
public class BuildStep : ExecutionStep
{
    /// <summary>
    /// Relative path to the project or solution to be built.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Build flavor used during the build process (e.g. Release or Debug).
    /// </summary>
    public BuildFlavor Flavor { get; set; } = BuildFlavor.Release;

    /// <summary>
    /// Build tool used to execute the build (e.g. MSBuild, Gradle, NPM).
    /// </summary>
    public BuildTool Tool { get; set; }

    /// <summary>
    /// Type of build operation to perform (e.g. Build or Rebuild).
    /// </summary>
    public BuildType BuildType { get; set; } = BuildType.Build;

    /// <summary>
    /// Target platform for the build (e.g. x64, AnyCPU).
    /// </summary>
    public string? Platform { get; set; } = string.Empty;

    /// <summary>
    /// Build target or task name (e.g. an NPM script name or a Gradle task).
    /// </summary>
    public string? Target { get; set; } = string.Empty;

    /// <summary>
    /// Visual Studio version used for the build, when applicable.
    /// </summary>
    public VisualStudioVersions? VsVersion { get; set; }

    /// <summary>
    /// Additional build parameters provided as key-value pairs.
    /// </summary>
    public Dictionary<string, string>? ExtraParams { get; set; } = new();

    /// <summary>
    /// Path to the output file produced by the build.
    /// </summary>
    public string? OutputFile { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether MSBuild is used explicitly for the build.
    /// </summary>
    public bool? UseMsBuild { get; set; } = false;
}
