using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;
using QuestPDF.Helpers;

namespace Lattice.Builders;

[Export(typeof(PageBuilder))]
[AutoConstructor]
public partial class PageBuilder
{
    private readonly IEnumerable<IPagePartBuilder> PageBuilders;
    private readonly TextStyleMutator TextStyleMutator;
    private readonly ColourConverter ColourConverter; 

    public void Build(Node node, IDocumentContainer documentContainer)
    {
        documentContainer.Page(page =>
        {
            var margin = node.GetAttributeFloat("margin");
            var marginBottom = node.GetAttributeFloat("marginBottom");
            var marginTop = node.GetAttributeFloat("marginTop");
            var marginRight = node.GetAttributeFloat("marginRight");
            var marginLeft = node.GetAttributeFloat("marginLeft");
            var background = node.GetAttribute("background");

            if (margin.HasValue)
                page.Margin(margin.Value);
            if (marginBottom.HasValue)
                page.MarginBottom(marginBottom.Value);
            if (marginTop.HasValue)
                page.MarginTop(marginTop.Value);
            if (marginRight.HasValue)
                page.MarginRight(marginRight.Value);
            if (marginLeft.HasValue)
                page.MarginLeft(marginLeft.Value);

            if (background is not null)
            {
                background = ColourConverter.ConvertToHex(background);
                page.PageColor(background);
            } 

            page.DefaultTextStyle(p => TextStyleMutator.Mutate(p, node));


            var pageSize = PageSizes.A4;
            
            var size = node.GetAttribute("size");
            if (size is not null)
            {
                var split = size.Split(",").Select(s => s.Trim()).ToArray();
                if (split.Length == 2)
                {
                    var hasWidth = int.TryParse(split[0], out var width);
                    var hasHeight = int.TryParse(split[1], out var height);
                    if (hasWidth && hasHeight)
                    {
                        pageSize = new PageSize(width, height, Unit.Inch);
                    }
                }
            }
            var orientation = node.GetAttribute("orientation");
            pageSize = orientation is "landscape" ? pageSize.Landscape() : pageSize;
            page.Size(pageSize);
            
            foreach (var childNode in node.ChildNodes)
            {
                var builder = PageBuilders.FirstOrDefault(b => b.Type == childNode.Type);
                builder?.Build(childNode, page);
            }
        });
    }
}