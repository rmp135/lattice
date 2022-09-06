using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class ExtendAttributeMutator : BaseAttributeMutator
{
    public override string Name => "extend";
    
    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (value == "horizontal")
            newContainer = newContainer.ExtendHorizontal();
        if (value == "vertical")
            newContainer = newContainer.ExtendVertical();
        if (value == "both")
            newContainer = newContainer.Extend();
        return newContainer;
    }
}