using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Execution;

namespace BuildHub.CommandBuilder.CommandBuilders;

using static BuildHub.Common.Utilities.StringUtilities;

/// <summary>
/// Command builder responsible for generating Git command-line arguments to fetch sources.
/// </summary>
public class GitCommandBuilder : CommandBuilderBase<GitCommandBuilder, GitContext>
{
	protected override string Name => "Git";

	public GitCommandBuilder(GitContext context)
		: base(context)
	{ }

	public GitCommandBuilder()
		: this(new GitContext())
	{ }

	protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
	{
		var commands = new List<ExecutionCommand>();

		if (Context.Clean)
		{
			commands.Add(new ExecutionCommand { Arguments = $"rmdir /s /q \"{Context.TargetDirectory}\"" });
		}

		// TODO: This will surely break the command builder
		commands.Add(new ExecutionCommand { Arguments = $"if not exist \"{Context.TargetDirectory}\" mkdir \"{Context.TargetDirectory}\"" });
		commands.Add(new ExecutionCommand { Arguments = $"if not exist \"{Context.TargetDirectory}\\.git\" git clone {AddQuotes(Context.Repository)} \"{Context.TargetDirectory}\" else git -C \"{Context.TargetDirectory}\" fetch" });

		if (!string.IsNullOrEmpty(Context.Version))
			commands.Add(new ExecutionCommand { Arguments = $"git -C \"{Context.TargetDirectory}\" checkout {Context.Version}" });
		else
			commands.Add(new ExecutionCommand { Arguments = $"git -C \"{Context.TargetDirectory}\" checkout {Context.Branch}" });

		commands.Add(new ExecutionCommand { Arguments = $"git -C \"{Context.TargetDirectory}\" pull" });

		return new List<ExecutionStep>
		{
			new ExecutionStep
			{
				Id = new Guid(),
				Order = 1,
				Name = Name ?? string.Empty,
				StepType = Name ?? string.Empty,
				Commands = commands
			}
		};
	}

	protected override void ValidateDerived()
	{
		if (string.IsNullOrEmpty(Context.Repository))
			throw new InvalidOperationException("Repository URL cannot be empty.");

		if (string.IsNullOrEmpty(Context.TargetDirectory))
			throw new InvalidOperationException("Target directory cannot be empty.");
	}

	public GitCommandBuilder SetRepository(string repository)
	{ Context.Repository = repository; return this; }

	public GitCommandBuilder SetBranch(string branch)
	{ Context.Branch = branch; return this; }

	public GitCommandBuilder SetVersion(string version)
	{ Context.Version = version; return this; }

	public GitCommandBuilder SetTargetDirectory(string targetDirectory)
	{ Context.TargetDirectory = targetDirectory; return this; }

	public GitCommandBuilder SetClean(bool clean)
	{ Context.Clean = clean; return this; }
}
