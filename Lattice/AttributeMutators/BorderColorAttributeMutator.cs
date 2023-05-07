using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Drawing;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;


[Export(typeof(IAttributeMutator))]
public class BorderColorAttributeMutator : IAttributeMutator
{

    public string Name => "borderColor";
    
    private readonly ColourConverter ColourColourConverter;
    private readonly ContextReplacer ContextReplacer;
    
    public BorderColorAttributeMutator(
        ColourConverter colourColourConverter,
        ContextReplacer contextReplacer
    )
    {
        ColourColourConverter = colourColourConverter;
        ContextReplacer = contextReplacer;
    }

    public IContainer Mutate(IContainer container, Node node)
    {
        var value = node.GetAttribute(Name);
        if (value is null) return container;
        value = ContextReplacer.ReplaceTokens(value, node);
        var convertedColour = ColourColourConverter.ConvertToHex(value);
        return container.BorderColor(convertedColour);
    }
}

