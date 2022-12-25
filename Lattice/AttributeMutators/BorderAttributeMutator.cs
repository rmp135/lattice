using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Drawing;

namespace Lattice.AttributeMutators;

public abstract class BaseBorderAttributeMutator : BaseAttributeMutator
{
    protected IContainer Mutate(IContainer container, string value, Func<float, Unit, IContainer> func)
    {
        return !int.TryParse(value, out var paddingValue) ? container : func(paddingValue, Unit.Point);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "border";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.Border);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderLeftAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "borderLeft";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.BorderLeft);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderRightAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "borderRight";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.BorderRight);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderTopAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "borderTop";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.BorderTop);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderBottomAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "borderBottom";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.BorderBottom);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderVerticalAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "borderVertical";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.BorderVertical);
    }
}

[Export(typeof(IAttributeMutator))]
public class BorderHorizontalAttributeMutator : BaseBorderAttributeMutator
{
    public override string Name => "borderHorizontal";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.BorderHorizontal);
    }
}

