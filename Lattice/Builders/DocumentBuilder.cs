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

[Export(typeof(IDocumentBuilder))]
internal class DocumentBuilder : IDocumentBuilder
{
    private readonly PageBuilder PageBuilder;

    public DocumentBuilder(PageBuilder pageBuilder)
    {
        PageBuilder = pageBuilder;
    }

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