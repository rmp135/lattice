using QuestPDF.Fluent;
using Lattice.Nodes;

namespace Lattice.Builders;

public interface IPagePartBuilder
{
    NodeType Type { get; }
    void Build(Node node, PageDescriptor page);
}