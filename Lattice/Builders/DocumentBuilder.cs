using QuestPDF.Fluent;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(DocumentBuilder))]
public class DocumentBuilder
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