using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;


[Export<IAttributeMutator>]
public class BorderColorAttributeMutator(
    ColourConverter ColourColourConverter,
    ContextReplacer ContextReplacer
)
    : IAttributeMutator
{

    public string Name => "borderColor";

    public IContainer Mutate(IContainer container, Node node)
    {
        var value = node.GetAttribute(Name);
        if (value is null) return container;
        value = ContextReplacer.ReplaceTokens(value, node);
        var convertedColour = ColourColourConverter.ConvertToHex(value);
        return container.BorderColor(convertedColour);
    }
}

