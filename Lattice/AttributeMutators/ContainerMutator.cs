using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;

[Export<ContainerMutator>]
public class ContainerMutator(
    IEnumerable<IAttributeMutator> AttributeMutators,
    TextStyleMutator TextStyleMutator
)
{
    public IContainer Mutate(IContainer container, Node node)
    {
        var newContainer = container.DefaultTextStyle(s => TextStyleMutator.Mutate(s, node));

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var attr in node.Attributes)
        {
            var foundMutator = AttributeMutators.FirstOrDefault(m => string.Equals(m.Name, attr.Key, StringComparison.InvariantCultureIgnoreCase));
            if (foundMutator is not null)
            {
                newContainer = foundMutator.Mutate(newContainer, node);
            }
        }

        return newContainer;
    }
}