using Lattice.Nodes;

namespace Lattice.Tests.NodeConstructorTests;

public class ExpandRepeaterTests : NodeConstructorTestsBase
{
    [Test]
    public async Task MultipleChildren()
    {
        var node = A.Fake<Node>();

        node.ChildNodes.Add(A.Dummy<Node>());
        node.ChildNodes.Add(A.Dummy<Node>());

        var callback = A.Fake<Func<Node, Task<IEnumerable<Node>>>>();
        var childNodes = A.CollectionOfDummy<Node>(2);
        A.CallTo(() => callback.Invoke(node.ChildNodes[0])).Returns(childNodes);

        var moreChildNodes = A.CollectionOfDummy<Node>(1);
        A.CallTo(() => callback.Invoke(node.ChildNodes[1])).Returns(moreChildNodes);

        await nodeConstructor.ExpandRepeaterAsync(node, callback);
        Assert.That(node.ChildNodes, Is.EquivalentTo(childNodes.Concat(moreChildNodes)));
    }

    [Test]
    public async Task NoChildren()
    {
        var node = A.Fake<Node>();
        
        var callback = A.Fake<Func<Node, Task<IEnumerable<Node>>>>();

        await nodeConstructor.ExpandRepeaterAsync(node, callback);
        Assert.That(node.ChildNodes, Is.Empty);
    }
}