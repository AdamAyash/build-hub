using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Execution;

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
{
    /// <summary>
    /// Logical name or identifier associated with the command being built.
    /// </summary>
    protected string Name { get; set; } = string.Empty;

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
        ValidateAll();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        BuildCommandResult result;
        IReadOnlyList<ExecutionStep> steps = new List<ExecutionStep>();

        try
        {
            steps = GenerateCommandInternal();
        }
        catch (Exception ex)
        {
            result = BuildCommandResult.OnFailure(Name, stopwatch.Elapsed, ex.Message);
        }

        stopwatch.Stop();

        return BuildCommandResult.OnSuccess(Name, stopwatch.Elapsed, steps);
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

    public void ValidateAll()
    {
        ValidateCore();       // CommandBuilderBase-level validation
        ValidateDerived();    // intermediate + concrete validations
    }

    protected virtual void ValidateCore() { }

    // This will be overridden by concrete builders
    protected abstract void ValidateDerived();

    /// <summary>
    /// Assigns a name or identifier to the command builder.
    /// </summary>
    /// <param name="name">Name associated with the command.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public virtual TBuilder SetName(string name)
    { Name = name; return Self; }

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

    /// <summary>
    /// Wraps a value in double quotes for safe use in command-line arguments.
    /// </summary>
    /// <param name="value">Value to be quoted.</param>
    /// <returns>The quoted value.</returns>
    public string AddQuotes(string value)
    {
        return $@"""{value}""";
    }
}
