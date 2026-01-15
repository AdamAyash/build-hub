using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Execution;

namespace BuildHub.CommandBuilder.CommandBuilders;

public class DotnetCommandBuilder : CommandBuilderBase<DotnetCommandBuilder, DotnetContext>
{
    public DotnetCommandBuilder(DotnetContext context)
        : base(context)
    {
        Name = "Dotnet";
    }

    public DotnetCommandBuilder()
        : this(new DotnetContext())
    {
    }

    protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
    {
        throw new NotImplementedException();
    }

    protected override void ValidateDerived()
    {
        throw new NotImplementedException();
    }
}
