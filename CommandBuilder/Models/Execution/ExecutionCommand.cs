namespace BuildHub.CommandBuilder.Models.Execution;

public sealed record ExecutionCommand
{
	/// <summary>Command executable (e.g. dotnet, msbuild, cmd)</summary>
	public string Executable { get; init; } = string.Empty;

	/// <summary>Arguments passed to the executable</summary>
	public string Arguments { get; init; } = string.Empty;

	/// <summary>Optional working directory override</summary>
	public string? WorkingDirectory { get; init; }
}
