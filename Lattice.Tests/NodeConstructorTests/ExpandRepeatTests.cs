using Lattice.Nodes;

namespace Lattice.Tests.NodeConstructorTests;

public class ExpandRepeatTests : NodeConstructorTestsBase
{
    [Test]
    public void NoAttribute_ReturnsSelf()
    {
        var node = new Node(NodeType.Column);
        
        var result = nodeConstructor.ExpandRepeat(node);
        
        Assert.That(result, Is.EquivalentTo(new[] { node }));
    }
    
    [Test]
    public void InvalidIntAttribute_ReturnsSelf()
    {
        var node = new Node(NodeType.Column);
        node.AddAttribute("repeat", "not an int");
        
        var result = nodeConstructor.ExpandRepeat(node);
        
        Assert.That(result, Is.EquivalentTo(new[] { node }));
    }
    
    [Test]
    public void ValidAttribute_ReturnsSelfMultiplied()
    {
        var node = new Node(NodeType.Column);
        node.AddAttribute("repeat", "2");
        
        var result = nodeConstructor.ExpandRepeat(node);
        var resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(2));
        
        Assert.That(resultArray[0].GetAttribute("repeat"), Is.Null);
        Assert.That(resultArray[1].Context, Contains.Key("$index"));
        Assert.That(resultArray[1].Context["$index"], Is.TypeOf<StringContextValue>());
        Assert.That(resultArray[0].Context["$index"].ToString(), Is.EqualTo("0"));
        
        Assert.That(resultArray[1].GetAttribute("repeat"), Is.Null);
        Assert.That(resultArray[1].Context, Contains.Key("$index"));
        Assert.That(resultArray[1].Context["$index"], Is.TypeOf<StringContextValue>());
        Assert.That(resultArray[1].Context["$index"].ToString(), Is.EqualTo("1"));
    }
    
    [Test]
    public void ValidAttribute_CallsDeepCloneEachTime()
    {
        var node = A.Fake<Node>();

        A.CallTo(() => node.GetAttribute("repeat")).Returns("2");
        var deepClone = A.CallTo(() => node.DeepClone());
        deepClone.Returns(node);
        
        nodeConstructor.ExpandRepeat(node);
        deepClone.MustHaveHappenedTwiceExactly();
    }
}