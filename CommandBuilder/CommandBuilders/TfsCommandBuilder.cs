using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Execution;

namespace BuildHub.CommandBuilder.CommandBuilders;

/// <summary>
/// Command builder responsible for generating TFS (tf.exe) commands to fetch sources.
/// </summary>
public class TfsCommandBuilder : CommandBuilderBase<TfsCommandBuilder, TfsContext>
{
	protected override string Name => "TFS";

	public TfsCommandBuilder(TfsContext context)
		: base(context)
	{ }

	public TfsCommandBuilder()
		: this(new TfsContext())
	{ }

	protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
	{
		var command = new List<string>
		{
			$"tf get \"{Context.WorkspacePath}\""
		};

		if (!string.IsNullOrEmpty(Context.Changeset))
			command.Add($"/version:C{Context.Changeset}");
		else
			command.Add("/version:T"); // latest

		if (Context.Recursive)
			command.Add("/recursive");

		if (Context.Force)
			command.Add("/force");

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
		if (string.IsNullOrEmpty(Context.WorkspacePath))
			throw new InvalidOperationException("Workspace path cannot be empty.");
	}

	/// <summary>
	/// Sets the workspace path to retrieve.
	/// </summary>
	public TfsCommandBuilder SetWorkspacePath(string workspacePath)
	{ Context.WorkspacePath = workspacePath; return this; }

	/// <summary>
	/// Sets the changeset number to retrieve.
	/// </summary>
	public TfsCommandBuilder SetChangeset(string changeset)
	{ Context.Changeset = changeset; return this; }

	/// <summary>
	/// Enables or disables recursive retrieval.
	/// </summary>
	public TfsCommandBuilder SetRecursive(bool recursive)
	{ Context.Recursive = recursive; return this; }

	/// <summary>
	/// Enables or disables forced file overwrite.
	/// </summary>
	public TfsCommandBuilder SetForce(bool force)
	{ Context.Force = force; return this; }
}
