using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class ShowOnceAttributeMutator : BaseAttributeMutator
{
    public override string Name => "showOnce";
    
    protected override IContainer Mutate(IContainer container, string value)
    {
        return container.ShowOnce();
    }
}