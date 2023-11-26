﻿using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using Lattice.Builders;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Lattice.Sources;
using Lattice.Web.Data;
using Lattice.Web.Data.Sources;
using Node = Lattice.Nodes.Node;

namespace Lattice.Web.Routes;

[Route("api/[controller]")]
public class PDFController : Controller
{
    private readonly INodeConstructor NodeConstructor;
    private readonly IDocumentBuilder DocumentBuilder;
    private readonly PreviewStorage _previewStorage;

    private readonly ISource _source = new CSVSource("Data/Sources/Elements.csv");
    
    public PDFController(INodeConstructor nodeConstructor, IDocumentBuilder documentBuilder, PreviewStorage previewStorage)
    {
        NodeConstructor = nodeConstructor;
        DocumentBuilder = documentBuilder;
        _previewStorage = previewStorage;
    }

    private Node NodeFromXML(XElement xmlNode)
    {
        var node = new Node(xmlNode.Name.LocalName);
        if (!string.IsNullOrEmpty(xmlNode.Value))
        {
            node.AddAttribute("text", xmlNode.Value);
        }
        foreach (var attribute in xmlNode.Attributes())
        {
            node.AddAttribute(attribute.Name.LocalName, attribute.Value);
        }

        foreach (var childNode in xmlNode.Elements())
        {
            node.AddChild(NodeFromXML(childNode));
        }
        return node;
    }

    [HttpPost("")]
    public async Task<IActionResult> IndexPost()
    {
        using var body = new StreamReader(Request.Body, Encoding.UTF8);
        var xml = await body.ReadToEndAsync();
        try
        {
            _ = XDocument.Parse(xml);
        }
        catch (XmlException xmlException)
        {
            return new BadRequestObjectResult(new
            {
                ColumnNumber = xmlException.LinePosition,
                xmlException.LineNumber,
                xmlException.Message
            });
        }

        var id =  _previewStorage.AddToStore(xml);
        return new OkObjectResult(id);
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string id)
    {
        var xmlAsString = _previewStorage.Get(id);
        if (xmlAsString is null) return new NotFoundResult();
        var document = XDocument.Parse(xmlAsString);

        var rootNode = NodeFromXML(document.Root);
        await NodeConstructor.ConstructAsync(rootNode, _source);
        var stream = new MemoryStream();
        DocumentBuilder.Build(rootNode).GeneratePdf(stream);
        stream.Position = 0;
        return new FileStreamResult(stream, "application/pdf");
    }
}