using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;

public interface IAttributeMutator
{
    string Name { get; }

    IContainer Mutate(IContainer container, Node node);
}

public abstract class BaseAttributeMutator : IAttributeMutator
{
    public abstract string Name { get; }

    public IContainer Mutate(IContainer container, Node node)
    {
        var value = node.GetAttribute(Name);
        return value is null ? container : Mutate(container, value);
    }

    protected abstract IContainer Mutate(IContainer container, string value);
}