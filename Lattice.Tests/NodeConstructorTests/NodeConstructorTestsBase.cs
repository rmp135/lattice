namespace Lattice.Tests.NodeConstructorTests;

public abstract class NodeConstructorTestsBase
{
    protected NodeConstructor nodeConstructor;
    protected ContextReplacer contextReplacer;
    
    [SetUp]
    public void Setup()
    {
        contextReplacer = A.Fake<ContextReplacer>();
        nodeConstructor = new NodeConstructor(contextReplacer);
    }
}