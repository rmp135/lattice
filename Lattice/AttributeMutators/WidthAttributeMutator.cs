using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

[Export<IAttributeMutator>]
public class WidthAttributeMutator : BaseAttributeMutator
{
    public override string Name => "width";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var height))
        {
            newContainer = newContainer.Width(height);
        }
        return newContainer;
    }
}

public class MaxWidthAttributeMutator : BaseAttributeMutator
{
    public override string Name => "maxWidth";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var height))
        {
            newContainer = newContainer.MaxWidth(height);
        }
        return newContainer;
    }
}

public class MinWidthAttributeMutator : BaseAttributeMutator
{
    public override string Name => "minWidth";

    protected override IContainer Mutate(IContainer container, string value)
    {
        var newContainer = container;
        if (float.TryParse(value, out var height))
        {
            newContainer = newContainer.MinWidth(height);
        }
        return newContainer;
    }
}