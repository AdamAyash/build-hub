namespace BuildHub.CommandBuilder.Models.Execution.Enums;

/// <summary>
/// Supported build tools for build steps.
/// </summary>
public enum BuildTool
{
    VisualStudio,
    MSBuild,
    IncrediBuild,
    DotNetCLI,
    Npm
}