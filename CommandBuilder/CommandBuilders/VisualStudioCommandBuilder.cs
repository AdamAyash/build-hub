using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Execution;

namespace BuildHub.CommandBuilder.CommandBuilders;

/// <summary>
/// Command builder responsible for generating VisualStudio command-line arguments.
/// </summary>
public class VisualStudioCommandBuilder : MsBuildBasedCommandBuilderBase<VisualStudioCommandBuilder, VisualStudioContext>
{
	public VisualStudioCommandBuilder(VisualStudioContext context)
		: base(context)
	{
		Name = "VisualStudio";
	}

	public VisualStudioCommandBuilder()
		: this(new VisualStudioContext())
	{ }

	protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
	{
		var command = new List<string>
		{
			$"/{Context.BuildType.ToString()}",
			AddQuotes(GetConfigurationPlatform()),
			AddQuotes(Context.Target)
		};

		if (!string.IsNullOrEmpty(Context.OutputFile))
		{
			command.Add("/Out");
			command.Add(AddQuotes(Context.OutputFile));
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

	protected override void ValidateDerived() { }

	/// <summary>
	/// Specifies the output log file path.
	/// </summary>
	public VisualStudioCommandBuilder SetOutputFile(string outputFile)
	{ Context.OutputFile = outputFile; return this; }
}
