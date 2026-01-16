using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Execution;

namespace BuildHub.CommandBuilder.CommandBuilders;

/// <summary>
/// Command builder responsible for generating MSBuild command-line arguments.
/// </summary>
public class MSBuildCommandBuilder : MsBuildBasedCommandBuilderBase<MSBuildCommandBuilder, MsBuildContext>
{
	public MSBuildCommandBuilder(MsBuildContext context)
		: base(context)
	{
		Name = "MSBuild";
	}

	public MSBuildCommandBuilder()
		: this(new MsBuildContext())
	{ }

	protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
	{
		var command = new List<string>
		{
			AddQuotes(Context.Target),
			$"/p:Configuration={AddQuotes(Context.Flavor.ToString())}",
			$"/p:Platform={AddQuotes(Context.Platform)}",
			$"/p:VersionAssembly={$"{Context.Version.GetStringVersion()}"}",
			$"/t:{Context.BuildType.ToString().ToLower()}",
			"-restore"
		};

		foreach (var item in _extraParams)
		{
			command.Add(item.Value);
		}

		return new List<ExecutionStep>
		{
			new ExecutionStep
			{
				Id = new Guid(),
				Order = 1,
				Name = Name ?? string.Empty,
				StepType = Name ?? string.Empty,
				Commands = new List<ExecutionCommand>
				{
					new ExecutionCommand
					{
						Executable = Name ?? string.Empty,
						Arguments = string.Join(" ", command),
					}
				}
			}
		};
	}

	protected override void ValidateDerived()
	{
		if (Context.Version == null)
		{ throw new InvalidOperationException("Build version cannot be empty."); }
	}

	/// <summary>
	/// Specifies the version applied to the build when using MSBuild.
	/// </summary>
	public MSBuildCommandBuilder SetVersion(BuildVersion version)
	{ Context.Version = version; return this; }

	/// <summary>
	/// Specifies the version applied to the build when using MSBuild.
	/// </summary>
	public MSBuildCommandBuilder SetVersion(string version)
	{ Context.Version = new BuildVersion().SetFromString(version); return this; }
}
