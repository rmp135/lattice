using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Drawing;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class BorderAttributeMutator : BaseAttributeMutator
{
    public override string Name => "border";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var border))
        {
            newContainer = newContainer.Border(border);
        }
        return newContainer;
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderColourAttributeMutator : BaseAttributeMutator
{
    private readonly ColourConverter ColourConverter;

    public BorderColourAttributeMutator(ColourConverter colourConverter)
    {
        ColourConverter = colourConverter;
    }

    public override string Name => "borderColour";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        var color = ColourConverter.ConvertToHex(value);
        newContainer = newContainer.BorderColor(color);
        return newContainer;
    }
}