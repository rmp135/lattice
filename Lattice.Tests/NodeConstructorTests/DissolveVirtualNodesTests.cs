using Lattice.Nodes;

namespace Lattice.Tests.NodeConstructorTests;

public class DissolveVirtualNodesTests : NodeConstructorTestsBase
{
    [Test]
    public void NoChildren_DoesNothing()
    {
        var node = new Node(NodeType.Column);
        
        nodeConstructor.DissolveVirtualNodes(node);
        
        Assert.That(node.ChildNodes, Is.Empty);
    }
    
    [Test]
    public void NoVirtualNodes_KeepsChildren()
    {
        var node = new Node(NodeType.Column);
        
        var childNode1 = new Node(NodeType.Column);
        node.ChildNodes.Add(childNode1);
        
        var childNode2 = new Node(NodeType.Column);
        node.ChildNodes.Add(childNode2);
        
        nodeConstructor.DissolveVirtualNodes(node);
        
        Assert.That(node.ChildNodes, Has.Count.EqualTo(2));
        Assert.That(node.ChildNodes, Is.EquivalentTo(new[] { childNode1, childNode2 }));
    }
    
    [Test]
    public void SingleVirtualNodeNoGrandchildren_RemovesVirtualNode()
    {
        var node = new Node(NodeType.Column);
        
        var childNode = new Node(NodeType.Virtual);
        node.ChildNodes.Add(childNode);
        
        nodeConstructor.DissolveVirtualNodes(node);
        
        Assert.That(node.ChildNodes, Is.Empty);
    }
    
    [Test]
    public void SingleVirtualNodeWithGrandchildren_Merges()
    {
        var node = new Node(NodeType.Column);
        
        var childNode = new Node(NodeType.Virtual);
        node.ChildNodes.Add(childNode);
        
        var gChildNode1 = new Node(NodeType.Column);
        childNode.ChildNodes.Add(gChildNode1);
        
        var gChildNode2 = new Node(NodeType.Column);
        childNode.ChildNodes.Add(gChildNode2);
        
        nodeConstructor.DissolveVirtualNodes(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(2));
        Assert.That(node.ChildNodes, Is.EquivalentTo(new[] { gChildNode1, gChildNode2 }));
    }
    
    [Test]
    public void MultipleVirtualNodeWithGrandchildren_Merges()
    {
        var node = new Node(NodeType.Column);
        
        var vNode1 = new Node(NodeType.Virtual);
        node.ChildNodes.Add(vNode1);
        
        var gChildNode1 = new Node(NodeType.Column);
        vNode1.ChildNodes.Add(gChildNode1);
        
        var vNode2 = new Node(NodeType.Virtual);
        node.ChildNodes.Add(vNode2);
        
        var gChildNode2 = new Node(NodeType.Column);
        vNode2.ChildNodes.Add(gChildNode2);
        
        nodeConstructor.DissolveVirtualNodes(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(2));
        Assert.That(node.ChildNodes, Is.EquivalentTo(new[] { gChildNode1, gChildNode2 }));
    }
    
    [Test]
    public void SingleVirtualNodeWithGrandchildren_KeepsContext()
    {
        var node = new Node(NodeType.Column);
        
        var childNode = new Node(NodeType.Virtual);
        childNode.Context.Add("test", new StringContextValue("value"));
        node.ChildNodes.Add(childNode);
        
        var gChildNode1 = new Node(NodeType.Column);
        childNode.ChildNodes.Add(gChildNode1);

        nodeConstructor.DissolveVirtualNodes(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(1));
        Assert.That(node.ChildNodes, Is.EquivalentTo(new[] { gChildNode1 }));
        Assert.That(gChildNode1.Context, Contains.Key("test"));
        Assert.That(gChildNode1.Context["test"], Is.InstanceOf<StringContextValue>());
        Assert.That(gChildNode1.Context["test"].ToString(), Is.EqualTo("value"));
    }
}