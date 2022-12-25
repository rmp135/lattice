using Lattice.Nodes;
using Moq;

namespace Lattice.Tests;
using Lattice;

public class NodeConstructorTests
{
    [Test]
    public async Task SingleChildRepeat()
    {
        var mockReplacer = new Mock<ContextReplacer>();
        var constructor = new NodeConstructor(mockReplacer.Object);
        var node = new Node(NodeType.Column);
        node.AddChild(new TextNode("test").AddAttribute("repeat", "2"));
        await constructor.ConstructAsync(node);
        Assert.That(node.ChildNodes, Has.Count.EqualTo(2));
        Assert.Pass();
    }
    [Test]
    public async Task MultipleChildRepeat()
    {
        var mockReplacer = new Mock<ContextReplacer>();
        var constructor = new NodeConstructor(mockReplacer.Object);
        var node = new Node(NodeType.Column);
        node.AddChild(new TextNode("test2").AddAttribute("repeat", "2"));
        node.AddChild(new TextNode("test3").AddAttribute("repeat", "3"));
        await constructor.ConstructAsync(node);
        Assert.Multiple(() =>
        {
            Assert.That(node.ChildNodes[0].Attributes.First(p => p.Key == "text").Value, Is.EqualTo("test2"));
            Assert.That(node.ChildNodes[1].Attributes.First(p => p.Key == "text").Value, Is.EqualTo("test2"));
            Assert.That(node.ChildNodes[2].Attributes.First(p => p.Key == "text").Value, Is.EqualTo("test3"));
            Assert.That(node.ChildNodes[3].Attributes.First(p => p.Key == "text").Value, Is.EqualTo("test3"));
            Assert.That(node.ChildNodes[4].Attributes.First(p => p.Key == "text").Value, Is.EqualTo("test3"));
            Assert.That(node.ChildNodes, Has.Count.EqualTo(5));
        });
        Assert.Pass();
    }
}