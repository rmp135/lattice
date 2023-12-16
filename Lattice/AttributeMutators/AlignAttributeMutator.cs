using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export<IAttributeMutator>]
public class AlignAttributeMutator : BaseAttributeMutator
{
    public override string Name => "align";
    
    protected override IContainer Mutate(IContainer container, string value)
    {
        var values = value.Split(" ");
        var newContainer = container;
        foreach (var direction in values)
        {
            newContainer = direction switch
            {
                "left" => newContainer.AlignLeft(),
                "right" => newContainer.AlignRight(),
                "top" => newContainer.AlignTop(),
                "bottom" => newContainer.AlignBottom(),
                "center" => newContainer.AlignCenter(),
                "middle" => newContainer.AlignMiddle(),
                _ => newContainer
            };
        }
        return newContainer;
    }
}