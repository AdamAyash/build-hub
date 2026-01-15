using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Execution;

namespace BuildHub.CommandBuilder.CommandBuilders;

public class NpmCommandBuilder : CommandBuilderBase<NpmCommandBuilder, NodeContext>
{
    public NpmCommandBuilder(NodeContext context)
        : base(context)
    {
        Name = "Npm";
    }

    public NpmCommandBuilder()
        : this(new NodeContext())
    { }

    protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
    {
        throw new NotImplementedException();
    }

    protected override void ValidateDerived()
    {
        throw new NotImplementedException();
    }
}
