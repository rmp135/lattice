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
internal class ExportAttribute<T>(ExportScope scope = ExportScope.Singleton) : ExportAttribute(typeof(T), scope);

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal class ExportAttribute(Type Type, ExportScope scope = ExportScope.Singleton) : Attribute
{
    public ExportScope Scope { get; } = scope;
    public Type Type { get; } = Type;
}