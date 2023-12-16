using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export<IAttributeMutator>]
public class ExtendAttributeMutator : BaseAttributeMutator
{
    public override string Name => "extend";
    
    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        newContainer = value switch
        {
            "horizontal" => newContainer.ExtendHorizontal(),
            "vertical" => newContainer.ExtendVertical(),
            "both" => newContainer.Extend(),
            _ => newContainer
        };
        return newContainer;
    }
}