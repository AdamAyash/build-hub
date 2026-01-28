using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Execution;
using BuildHub.Common.Logger;

namespace BuildHub.CommandBuilder.CommandBuilders.Abstractions;

/// <summary>
/// Base class for command builders, providing shared state and
/// common helper functionality.
/// </summary>
/// <remarks>
/// This class uses a self-referencing generic type parameter to support
/// fluent APIs across inheritance hierarchies.
/// </remarks>
public abstract class CommandBuilderBase<TBuilder, TContext> : ICommandBuilder
	where TBuilder : CommandBuilderBase<TBuilder, TContext>
	where TContext : ICommandBuilderContext
{
	/// <summary>
	/// Logical name or identifier associated with the command being built.
	/// </summary>
	protected abstract string Name { get; }

	/// <summary>
	/// Additional command parameters provided as key-value pairs.
	/// </summary>
	protected Dictionary<string, string> _extraParams { get; set; } = new();

	/// <summary>
	/// Returns the current instance cast to the concrete builder type,
	/// enabling type-safe fluent method chaining in derived builders.
	/// </summary>
	protected TBuilder Self => (TBuilder)this;

	/// <summary>
	/// The context object containing environment, configuration, or shared data
	/// required to build the command.
	/// </summary>
	protected TContext Context;

	public CommandBuilderBase(TContext context)
	{
		Context = context ?? throw new ArgumentNullException(nameof(context));
	}

	public BuildCommandResult GenerateCommand()
	{
		try
		{
            Validate();
        }
		catch(Exception exception)
		{
			Logger.LogWarning(exception, $"Validation for {Name} failed.");
		}

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		IReadOnlyList<ExecutionStep> steps = new List<ExecutionStep>();

		try
		{

            steps = GenerateCommandInternal();
			
			return BuildCommandResult.CreateSuccess(Name, stopwatch.Elapsed, steps);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error generating command in {BuilderName}", Name);
			return BuildCommandResult.CreateFailure(Name, stopwatch.Elapsed, ex.Message);
		}
		finally
		{
			stopwatch.Stop();
		}
	}

	/// <summary>
	/// Generates the execution plan steps produced by this builder.
	/// </summary>
	/// <remarks>
	/// Implementations should return steps in the order they must be executed.
	/// Validation is assumed to have already occurred.
	/// </remarks>
	/// <returns>
	/// An ordered, read-only list of <see cref="ExecutionStep"/> objects.
	/// </returns>
	protected abstract IReadOnlyList<ExecutionStep> GenerateCommandInternal();

	public void Validate()
	{
		ValidateCore();       // CommandBuilderBase-level validation
		ValidateDerived();    // intermediate + concrete validations
	}

	protected virtual void ValidateCore() { }

	// This will be overridden by concrete builders
	protected abstract void ValidateDerived();

	/// <summary>
	/// Assigns additional command parameters used during command generation.
	/// </summary>
	/// <param name="extraParams">Key-value pairs representing extra parameters.</param>
	/// <returns>The current builder instance for fluent chaining.</returns>
	public virtual TBuilder SetExtraParams(Dictionary<string, string> extraParams)
	{ _extraParams = extraParams; return Self; }

	/// <summary>
	/// Sets the execution context used by this command builder.
	/// </summary>
	/// <param name="context">
	/// The context object containing environment, configuration, or shared data
	/// required to build the command.
	/// </param>
	/// <returns>The current builder instance to allow fluent method chaining.</returns>
	public virtual TBuilder SetContext(TContext context)
	{ Context = context; return Self; }
}
