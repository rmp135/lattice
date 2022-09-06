using QuestPDF.Previewer;
using Lattice.Builders;
using Lattice.Nodes;
using Lattice.Sources;

namespace Lattice;

[Export(typeof(Main))]
public class Main
{
    private readonly DocumentBuilder _documentBuilder;
    private readonly NodeConstructor _nodeConstructor;

    public Main(DocumentBuilder documentBuilder, NodeConstructor nodeConstructor)
    {
        _documentBuilder = documentBuilder;
        _nodeConstructor = nodeConstructor;
    }

    public async Task RunAsync()
    {
        // var e = new Expression("IF(index % 2 == 0, 1, 0)");
        // e.Bind("index", 3);
        // Console.WriteLine(e.Eval<bool>());
        var rootNode =
            new Node(NodeType.Document)
                .AddChild(
                    new Node(NodeType.Page)
                    .AddAttribute("for", "row in Tag")
                    .AddAttribute("background", "#e0e0e0")
                    .AddAttribute("margin", "10")
                    .AddAttribute("size", "6,4")
                    .AddChild(
                        new Node(NodeType.Footer)
                            .AddAttribute("align", "center")
                            .AddChild(new TextNode("{currentpage} / {totalpages}"))
                    )
                    .AddChild(
                        new Node(NodeType.Content)
                            .AddChild(
                                new Node(NodeType.Row)
                                    .AddAttribute("extend", "vertical")
                                    .AddAttribute("align", "center-middle")
                                    .AddChild(
                                        new Node(NodeType.Column)
                                            .AddChild(
                                                new TextNode("{row.ID}")
                                                    .AddAttribute("fontFamily", "Libre Barcode 39")
                                                    .AddAttribute("fontSize", "90")
                                                    .AddAttribute("align", "center")
                                            )
                                            .AddChild(
                                                new TextNode("{row.ID}")
                                                    .AddAttribute("fontSize", "20")
                                                    .AddAttribute("align", "center")
                                            )
                                            .AddChild(
                                                new TextNode("{row.Name}")
                                                    .AddAttribute("fontSize", "12")
                                                    .AddAttribute("align", "center")
                                            )
                                    )
                            )
                    )
                );
        await _nodeConstructor.ConstructAsync(rootNode);
        _documentBuilder.Build(rootNode).ShowInPreviewer();
    }
}