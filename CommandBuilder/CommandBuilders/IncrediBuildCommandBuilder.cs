using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Enums;
using BuildHub.CommandBuilder.Models.Execution;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace BuildHub.CommandBuilder.CommandBuilders;

using static BuildHub.Common.Utilities.StringUtilities;

/// <summary>
/// Command builder responsible for generating IncrediBuild command-line arguments.
/// </summary>
public class IncrediBuildCommandBuilder : MsBuildBasedCommandBuilderBase<IncrediBuildCommandBuilder, IncrediBuildContext>
{
	protected override string Name => "IncrediBuild";

	public IncrediBuildCommandBuilder(IncrediBuildContext context)
		: base(context)
	{ }

	public IncrediBuildCommandBuilder()
		: this(new IncrediBuildContext())
	{ }

	protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
	{
		var command = new List<string>
		{
			AddQuotes(Context.Target),
			$"/Cfg={AddQuotes(GetConfigurationPlatform())}",
			$"/Log={AddQuotes(Context.OutputFile)}",
			"/All"
		};

		if (Context.BuildType == BuildType.Rebuild)
			command.Add("/Rebuild");

		command.Add($"/VsVersion={VisualStudioInfo.GetCodeName(Context.VisualStudioVersion)}");

		if (Context.UseMsBuild)
		{
			var msCommand = new List<string>
			{
				$"/p:VersionAssembly={$"{Context.Version.GetStringVersion()}"}",
				$"/t:{Context.BuildType.ToString().ToLower()}",
				"-restore"
			};

			foreach (var item in _extraParams)
			{
				msCommand.Add(item.Value);
			}

			command.Add("/UseMSBuild");
			command.Add($"/msbuildargs={AddQuotes(string.Join(" ", msCommand))}");
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
		if (string.IsNullOrEmpty(Context.OutputFile))
		{ throw new InvalidOperationException("Build output file cannot be empty."); }

		if (Context.UseMsBuild)
		{
			if (Context.Version == null)
			{ throw new InvalidOperationException("Build version cannot be empty."); }
		}
	}

	/// <summary>
	/// Specifies the output log file path.
	/// </summary>
	public IncrediBuildCommandBuilder SetOutputFile(string outputFile)
	{ Context.OutputFile = outputFile; return this; }

	/// <summary>
	/// Specifies the version applied to the build when using MSBuild.
	/// </summary>
	public IncrediBuildCommandBuilder SetVersion(BuildVersion version)
	{ Context.Version = version; return this; }

	/// <summary>
	/// Specifies the version applied to the build when using MSBuild.
	/// </summary>
	public IncrediBuildCommandBuilder SetVersion(string version)
	{ Context.Version = new BuildVersion(version); return this; }

	/// <summary>
	/// Specifies the Visual Studio version used by IncrediBuild.
	/// </summary>
	public IncrediBuildCommandBuilder SetVisualStudioVersion(VisualStudioVersions visualStudioVersion)
	{ Context.VisualStudioVersion = visualStudioVersion; return this; }

	/// <summary>
	/// Controls whether MSBuild is used explicitly during the build.
	/// </summary>
	public IncrediBuildCommandBuilder SetUseMsBuild(bool useMsBuild)
	{ Context.UseMsBuild = useMsBuild; return this; }
}
