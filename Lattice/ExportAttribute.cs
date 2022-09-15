namespace Lattice;

public enum ExportScope
{
    Transient,
    Singleton
}

/// <summary>
/// Internal helper attribute that will auto-import into the DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal class ExportAttribute : Attribute
{
    public Type Type { get; }
    private ExportScope Scope { get; }

    public ExportAttribute(Type type, ExportScope scope = ExportScope.Singleton)
    {
        Type = type;
        Scope = scope;
    }
}