using Lattice.Nodes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class UnconstrainedAttributeMutator : IAttributeMutator
{
    public string Name => "unconstrained";

    public IContainer Mutate(IContainer container, Node node)
    {
        return container.Unconstrained();
    }
}