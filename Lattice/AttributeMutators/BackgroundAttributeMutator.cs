using Lattice.Nodes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class BackgroundAttributeMutator : IAttributeMutator
{
    public string Name => "background";

    private readonly ColourConverter ColourColourConverter;
    private readonly ContextReplacer ContextReplacer;

    public BackgroundAttributeMutator(ColourConverter colourConverter, ContextReplacer contextReplacer)
    {
        ColourColourConverter = colourConverter;
        ContextReplacer = contextReplacer;
    }

    public IContainer Mutate(IContainer container, Node node)
    {
        var value = node.GetAttribute(Name);
        if (value is null) return container;
        value = ContextReplacer.ReplaceTokens(value, node);
        var convertedColour = ColourColourConverter.ConvertToHex(value);
        return container.Background(convertedColour);
    }
}