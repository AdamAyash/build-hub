using BuildHub.CommandBuilder.CommandBuilders.Abstractions;

namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context for .NET CLI command builder.
/// Holds properties specific to dotnet commands (build, test, restore, etc.).
/// </summary>
public sealed class DotnetContext : ICommandBuilderContext
{
}
