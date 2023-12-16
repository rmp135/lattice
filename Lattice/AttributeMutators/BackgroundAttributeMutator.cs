using Lattice.Nodes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export<IAttributeMutator>]
public class BackgroundAttributeMutator(ColourConverter ColourConverter, ContextReplacer ContextReplacer)
    : IAttributeMutator
{
    public string Name => "background";

    public IContainer Mutate(IContainer container, Node node)
    {
        var value = node.GetAttribute(Name);
        if (value is null) return container;
        value = ContextReplacer.ReplaceTokens(value, node);
        var convertedColour = ColourConverter.ConvertToHex(value);
        return container.Background(convertedColour);
    }
}