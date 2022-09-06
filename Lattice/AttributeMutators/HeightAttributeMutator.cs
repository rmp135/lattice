using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export(typeof(IAttributeMutator))]
public class HeightAttributeMutator : BaseAttributeMutator
{
    public override string Name => "height";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var height))
        {
            newContainer = newContainer.Height(height);
        }
        return newContainer;
    }
}

[Export(typeof(IAttributeMutator))]
public class MaxHeightAttributeMutator : BaseAttributeMutator
{
    public override string Name => "maxHeight";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var height))
        {
            newContainer = newContainer.MaxHeight(height);
        }
        return newContainer;
    }
}

[Export(typeof(IAttributeMutator))]
public class MinHeightAttributeMutator : BaseAttributeMutator
{
    public override string Name => "minHeight";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var height))
        {
            newContainer = newContainer.MinHeight(height);
        }
        return newContainer;
    }
}