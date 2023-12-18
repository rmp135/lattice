using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using Lattice.Builders;
using System.Text;
using Lattice.Sources;
using Lattice.Web.Data;
using Lattice.Web.Data.Sources;
using Node = Lattice.Nodes.Node;
using HtmlAgilityPack;

namespace Lattice.Web.Routes;

[Route("api/[controller]")]
public class PDFController(
    INodeConstructor NodeConstructor,
    IDocumentBuilder DocumentBuilder,
    PreviewStorage PreviewStorage
) : Controller
{
    private readonly ISource _source = new CSVSource("Data/Sources/Elements.csv");

    private Node NodeFromXML(HtmlNode xmlNode)
    {
        var node = new Node(xmlNode.Name);
        if (!string.IsNullOrEmpty(xmlNode.InnerText))
        {
            node.AddAttribute("text", xmlNode.InnerText);
        }
        foreach (var attribute in xmlNode.Attributes)
        {
            node.AddAttribute(attribute.Name, attribute.Value);
        }

        foreach (var childNode in xmlNode.ChildNodes.Where(x => x.NodeType == HtmlNodeType.Element))
        {
            node.AddChild(NodeFromXML(childNode));
        }
        return node;
    }

    [HttpPost("")]
    public async Task<IActionResult> IndexPost()
    {
        using var body = new StreamReader(Request.Body, Encoding.UTF8);
        var bodyString = await body.ReadToEndAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(bodyString);
        if (doc.ParseErrors.Any())
        {
            var xmlException = doc.ParseErrors.First();
            return new BadRequestObjectResult(
                new
                {
                    ColumnNumber = xmlException.LinePosition,
                    LineNumber = xmlException.Line,
                    Message = xmlException.Reason
                }
            );
        }

        var id =  PreviewStorage.AddToStore(bodyString);
        return new OkObjectResult(id);
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string id)
    {
        var xmlAsString = PreviewStorage.Get(id);
        if (xmlAsString is null) return new NotFoundResult();
        var doc = new HtmlDocument();
        doc.LoadHtml(xmlAsString);
        var rootNode = NodeFromXML(doc.DocumentNode.FirstChild);
        await NodeConstructor.ConstructAsync(rootNode, _source);
        var stream = new MemoryStream();
        DocumentBuilder.Build(rootNode).GeneratePdf(stream);
        stream.Position = 0;
        return new FileStreamResult(stream, "application/pdf");
    }
}