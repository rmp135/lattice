using Lattice.Nodes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class MinimalBoxAttributeMutator : IAttributeMutator
{
    public string Name => "minimalbox";

    public IContainer Mutate(IContainer container, Node node)
    {
        return container.MinimalBox();
    }
}