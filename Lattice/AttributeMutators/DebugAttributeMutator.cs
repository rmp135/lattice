using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export<IAttributeMutator>]
public class DebugAttributeMutator : BaseAttributeMutator
{
    public override string Name => "debug";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return container.DebugArea(value);
    }
}