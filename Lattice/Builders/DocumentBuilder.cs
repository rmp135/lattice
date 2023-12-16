using QuestPDF.Fluent;
using Lattice.Nodes;

namespace Lattice.Builders;

public interface IDocumentBuilder
{
    /// <summary>
    /// Builds a complete <see cref="Document"/> from a node tree.
    /// </summary>
    /// <param name="node">The root <see cref="Node"/>. Should be of type <see cref="NodeType.Document"/>.</param>
    /// <returns>The <see cref="Document"/> for generating the PDF.</returns>
    Document Build(Node node);
}

[Export<IDocumentBuilder>]
internal class DocumentBuilder(PageBuilder PageBuilder) : IDocumentBuilder
{
    public Document Build(Node node)
    {
        var doc = Document.Create(document =>
        {
            foreach (var childNode in node.ChildNodes)
            {
                PageBuilder.Build(childNode, document);
            }
        });
        return doc;
    }
}