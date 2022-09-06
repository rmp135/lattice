using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;
using QuestPDF.Elements.Table;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
public class TableBuilder : IContainerBuilder
{
    private readonly ContainerBuilder ContainerBuilder;
    private readonly ContainerMutator ContainerMutator;

    public NodeType Type => NodeType.Table;

    public TableBuilder(ContainerBuilder containerBuilder, ContainerMutator containerMutator)
    {
        ContainerBuilder = containerBuilder;
        ContainerMutator = containerMutator;
    }

    public void Build(Node node, IContainer container)
    {
        var newContainer = ContainerMutator.Mutate(container, node);
        if (!node.ChildNodes.Any()) return;
        newContainer.Table(table =>
        {
            // Of our direct children (Row, Header or Footer), select the first of each direct children (column cells) grouped on index.
            var allcells = node.ChildNodes
                .Select(c => c.ChildNodes.Select((cc, i) => new { i, cc }))
                .SelectMany(a => a)
                .GroupBy(c => c.i)
                .Select(c => c.First().cc)
                .ToArray();

            table.ColumnsDefinition(c =>
            {
                if (!allcells.Any())
                {
                    // If no cells are found, make a placeholder column to prevent exception.
                    c.RelativeColumn();
                    return;
                }
                foreach (var colNode in allcells)
                {
                    var widthAttr = colNode.GetAttribute("columnWidth");
                    var widthRegex = new Regex(@"(relative|constant)\((\d+\.?\d*)\)|relative");
                    var matches = widthRegex.Matches(widthAttr ?? "");
                    if (!matches.Any())
                    {
                        c.RelativeColumn();
                        continue;
                    };

                    var columnType = matches.First().Groups[1];
                    var columnWidth = matches.First().Groups[2];

                    if (columnType.Value.Equals("relative", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (columnWidth.Success)
                        {
                            c.RelativeColumn(float.Parse(columnWidth.Value));
                            continue;
                        } 
                        c.RelativeColumn();
                    }
                    else if (columnType.Value.Equals("constant", StringComparison.InvariantCultureIgnoreCase))
                    {
                        c.ConstantColumn(float.Parse(columnWidth.Value));
                    }
                    else
                    {
                        c.RelativeColumn();
                    }
                }
            });
            var headers = node.ChildNodes.FirstOrDefault(c => c.Type == NodeType.TableHeader);
            if (headers is not null && headers.ChildNodes.Any())
            {
                foreach (var childNode in headers.ChildNodes)
                {
                    table.Header(h =>
                    {
                        ContainerBuilder.BuildSingleNode(childNode, ContainerMutator.Mutate(MutateTableCell(h.Cell(), childNode), headers));
                    });
                }
            }
            var footers = node.ChildNodes.FirstOrDefault(c => c.Type == NodeType.TableFooter);
            if (footers is not null && footers.ChildNodes.Any())
            {
                foreach (var childNode in footers.ChildNodes)
                {
                    table.Footer(h =>
                    {
                        ContainerBuilder.BuildSingleNode(childNode, ContainerMutator.Mutate(MutateTableCell(h.Cell(), childNode), footers));
                    });
                }
            }
            var rows = node.ChildNodes.Where(c => c.Type == NodeType.TableRow).ToArray();
            foreach (var row in rows)
            {
                foreach (var childNode in row.ChildNodes)
                {
                    ContainerBuilder.BuildSingleNode(childNode, ContainerMutator.Mutate(MutateTableCell(table.Cell(), childNode), row));
                }
            }
        });
    }

    private IContainer MutateTableCell(ITableCellContainer container, Node node)
    {
        var newContainer = container;
        var columnSpan = node.GetAttributeInt("columnSpan");
        if (columnSpan.HasValue)
        {
            newContainer = container.ColumnSpan((uint)columnSpan.Value);
        }
        var rowSpan = node.GetAttributeInt("rowSpan");
        if (rowSpan.HasValue)
        {
            newContainer = container.RowSpan((uint)rowSpan.Value);
        }
        return newContainer;
    }
}