using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Lattice.AttributeMutators;

public abstract class BasePaddingAttributeMutator : BaseAttributeMutator
{
    protected IContainer Mutate(IContainer container, string value, Func<float, Unit, IContainer> func)
    {
        return !int.TryParse(value, out var paddingValue) ? container : func(paddingValue, Unit.Point);
    }
}

[Export(typeof(IAttributeMutator))]
public class PaddingAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "padding";

    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.Padding);
    }
}

[Export(typeof(IAttributeMutator))]
public class PaddingTopAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "paddingTop";
    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.PaddingTop);
    }
}

[Export(typeof(IAttributeMutator))]
public class PaddingBottomAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "paddingBottom";
    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.PaddingBottom);
    }
}

[Export(typeof(IAttributeMutator))]
public class PaddingHorizontalAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "paddingHorizontal";
    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.PaddingHorizontal);
    }
}

[Export(typeof(IAttributeMutator))]
public class PaddingVerticalAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "paddingVertical";
    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.PaddingVertical);
    }
}

[Export(typeof(IAttributeMutator))]
public class PaddingLeftAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "paddingLeft";
    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.PaddingLeft);
    }
}


[Export(typeof(IAttributeMutator))]
public class PaddingRightAttributeMutator : BasePaddingAttributeMutator
{
    public override string Name => "paddingRight";
    protected override IContainer Mutate(IContainer container, string value)
    {
        return Mutate(container, value, container.PaddingRight);
    }
}