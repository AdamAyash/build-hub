namespace BuildHub.CommandBuilder.CommandBuilders.Abstractions;

/// <summary>
/// Marker interface for context objects used by <see cref="ICommandBuilder"/> implementations.
/// </summary>
/// <remarks>
/// This interface does not define any members. It is used solely to provide type safety and
/// to ensure that only valid command builder context types can be supplied to generic
/// <see cref="CommandBuilderBase{TBuilder, TContext}"/> implementations.
/// </remarks>
public interface ICommandBuilderContext
{
}