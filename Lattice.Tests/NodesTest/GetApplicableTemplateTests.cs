using Lattice.Nodes;

namespace Lattice.Tests.NodesTest;

public class GetApplicableTemplateTests
{
    [Test]
    public void NoParentNoTemplates()
    {
        var node = new Node(NodeType.Document);
        var templates = node.GetApplicableTemplateNodes();
        Assert.That(templates, Is.Empty);
    }

    [Test]
    public void NoSiblingsNoTemplates()
    {
        var parentNode = new Node(NodeType.Document);
        var node = new Node(NodeType.Document);
        parentNode.AddChild(node);
        var templates = node.GetApplicableTemplateNodes();
        Assert.That(templates, Is.Empty);
    }

    [Test]
    public void DirectSiblingHasTemplate()
    {
        var parentNode = new Node(NodeType.Document);
        var node = new Node(NodeType.Document);
        var siblingNode = new Node(NodeType.Template);
        parentNode.AddChild(siblingNode);
        parentNode.AddChild(node);
        var templates = node.GetApplicableTemplateNodes();
        Assert.That(templates, Is.EquivalentTo(new[] { siblingNode }));
    }

    [Test]
    public void ParentSiblingHasTemplate()
    {
        var grandparentNode = new Node(NodeType.Document);
        var parentNode = new Node(NodeType.Document);
        grandparentNode.AddChild(parentNode);
        var node = new Node(NodeType.Document);
        var templateNode = new Node(NodeType.Template);
        grandparentNode.AddChild(templateNode);
        grandparentNode.AddChild(parentNode);
        parentNode.AddChild(node);
        var templates = node.GetApplicableTemplateNodes();
        Assert.That(templates, Is.EquivalentTo(new[] { templateNode }));
    }

    [Test]
    public void SiblingAndParentSiblingHasTemplate()
    {
        var grandparentNode = new Node(NodeType.Document);
        var parentNode = new Node(NodeType.Document);
        grandparentNode.AddChild(parentNode);
        var node = new Node(NodeType.Document);
        var templateNode = new Node(NodeType.Template);
        var templateNode2 = new Node(NodeType.Template);
        grandparentNode.AddChild(templateNode);
        grandparentNode.AddChild(parentNode);
        parentNode.AddChild(node);
        parentNode.AddChild(templateNode2);
        var templates = node.GetApplicableTemplateNodes();
        Assert.That(templates, Is.EquivalentTo(new[] { templateNode, templateNode2 }));
    }
}