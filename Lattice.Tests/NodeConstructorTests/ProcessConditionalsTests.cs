using Lattice.Nodes;

namespace Lattice.Tests.NodeConstructorTests;

public class ProcessConditionalTests : NodeConstructorTestsBase
{
    [Test]
    public void NoChildren_DoesNotRemove()
    {
        var node = A.Fake<Node>();
        
        nodeConstructor.ProcessConditionals(node);
        Assert.That(node.ChildNodes, Is.Empty);
    }

    [Test]
    public void SingleIfReturnsTrue_DoesNotRemove()
    {
        var node = new Node(NodeType.Column);

        var childNode = new Node(NodeType.Column);
        childNode.AddAttribute("if", "x");
        node.ChildNodes.Add(childNode);
        
        A.CallTo(() => contextReplacer.ReplaceToken(childNode, "if(x, true, false)"))
            .Returns("True");
        
        nodeConstructor.ProcessConditionals(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(1));
        Assert.That(node.ChildNodes[0], Is.EqualTo(childNode));
    }

    [Test]
    public void SingleIfReturnsFalse_Removes()
    {
        var node = new Node(NodeType.Column);

        var childNode = new Node(NodeType.Column);
        childNode.AddAttribute("if", "x");
        node.ChildNodes.Add(childNode);
        
        nodeConstructor.ProcessConditionals(node);
        
        Assert.That(node.ChildNodes, Is.Empty);
    }
    
    [Test]
    public void MultipleIfReturnsFalseThenTrue_RemovesFalseOnly()
    {
        var node = new Node(NodeType.Column);

        var childNode1 = new Node(NodeType.Column);
        childNode1.AddAttribute("if", "x");
        node.ChildNodes.Add(childNode1);
        
        var childNode2 = new Node(NodeType.Column);
        childNode2.AddAttribute("if", "y");
        node.ChildNodes.Add(childNode2);
        
        A.CallTo(() => contextReplacer.ReplaceToken(childNode1, "if(x, true, false)"))
            .Returns("False");
        
        A.CallTo(() => contextReplacer.ReplaceToken(childNode2, "if(y, true, false)"))
            .Returns("True");
        
        nodeConstructor.ProcessConditionals(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(1));
        Assert.That(node.ChildNodes[0], Is.EqualTo(childNode2));
    }

    [Test]
    public void SingleIfReturnsFalseAndElse_RemovesIfOnly()
    {
        var node = new Node(NodeType.Column);

        var ifNode = new Node(NodeType.Column);
        ifNode.AddAttribute("if", "x");
        node.ChildNodes.Add(ifNode);
        
        var elseNode = new Node(NodeType.Column);
        elseNode.AddAttribute("else", "");
        node.ChildNodes.Add(elseNode);
        
        nodeConstructor.ProcessConditionals(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(1));
        Assert.That(node.ChildNodes[0], Is.EqualTo(elseNode));
    }

    [Test]
    public void SingleIfReturnsTrueAndElse_RemovesElse()
    {
        var node = new Node(NodeType.Column);

        var ifNode = new Node(NodeType.Column);
        ifNode.AddAttribute("if", "x");
        node.ChildNodes.Add(ifNode);
        
        var elseNode = new Node(NodeType.Column);
        elseNode.AddAttribute("else", "");
        node.ChildNodes.Add(elseNode);
        
        A.CallTo(() => contextReplacer.ReplaceToken(ifNode, "if(x, true, false)"))
            .Returns("True");
        
        nodeConstructor.ProcessConditionals(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(1));
        Assert.That(node.ChildNodes[0], Is.EqualTo(ifNode));
    }

    [Test]
    public void SingleElse_DoesNotRemove()
    {
        var node = new Node(NodeType.Column);

        var childNode = new Node(NodeType.Column);
        childNode.AddAttribute("else", "x");
        node.ChildNodes.Add(childNode);
        
        nodeConstructor.ProcessConditionals(node);
        
        Assert.That(node.ChildNodes.Count, Is.EqualTo(1));
        Assert.That(node.ChildNodes[0], Is.EqualTo(childNode));
    }
}