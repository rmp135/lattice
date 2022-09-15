using QuestPDF.Fluent;
using Lattice.Nodes;

namespace Lattice.Builders;

/// <summary>
/// Handles building parts of a page. <see cref="NodeType.Header"/>, <see cref="NodeType.Footer"/> and <see cref="NodeType.Content"/>.
/// </summary>
public interface IPagePartBuilder
{
    NodeType Type { get; }
    void Build(Node node, PageDescriptor page);
}