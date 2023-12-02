using Lattice.Nodes;
using Lattice.Sources;

namespace Lattice.Tests.NodeConstructorTests;

public class BindTests : NodeConstructorTestsBase
{
    [Test]
    public async Task NoBindKey()
    {
        var node = A.Fake<Node>();
        var source = A.Fake<ISource>();
        A.CallTo(() => node.GetAttribute("bind")).Returns(null);
        
        await nodeConstructor.BindAsync(node, source);

        Assert.That(node.Context, Is.Empty);
    }

    [Test]
    public async Task InvalidBindKey()
    {
        var node = A.Fake<Node>();
        var source = A.Fake<ISource>();

        A.CallTo(() => node.GetAttribute("bind")).Returns("invalid");
        
        await nodeConstructor.BindAsync(node, source);
        
        Assert.That(node.Context, Is.Empty);
    }
}