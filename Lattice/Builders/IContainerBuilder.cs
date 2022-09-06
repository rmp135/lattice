using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.Builders;

public interface IContainerBuilder
{
    public NodeType Type { get; }
    void Build(Node node, IContainer container);
}