using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;

public interface IAttributeMutator
{
    /// <summary>
    /// Case insensitive name of the attribute.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns a new container with the attribution applied.
    /// </summary>
    /// <param name="container">The <see cref="IContainer"/> to add the attribute to.</param>
    /// <param name="node">The <see cref="Node"/> that contains the container.</param>
    /// <returns>A container with the attribute applied. May not be the same reference as the parameter.</returns>
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