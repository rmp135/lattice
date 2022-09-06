namespace Lattice;

public enum ExportScope
{
    Transient,
    Singleton
}

public class ExportAttribute : Attribute
{
    public Type Type { get; }
    public ExportScope Scope { get; }

    public ExportAttribute(Type type, ExportScope scope = ExportScope.Transient)
    {
        Type = type;
        Scope = scope;
    }
}