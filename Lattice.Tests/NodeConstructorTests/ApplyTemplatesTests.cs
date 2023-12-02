using Lattice.Nodes;

namespace Lattice.Tests.NodeConstructorTests;

public class ApplyTemplatesTests : NodeConstructorTestsBase
{
    [Test]
    public void NoTemplateNoChange()
    {
        var node = A.Fake<Node>(o => o.Wrapping(new Node(NodeType.Document)));
        node.AddAttribute("test", "value");
        nodeConstructor.ApplyTemplates(node);
        A.CallTo(() => node.GetApplicableTemplateNodes()).MustNotHaveHappened();
        Assert.That(node.Attributes, Has.Count.EqualTo(1));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("test"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("value"));
    }

    
    [Test]
    public void NoTemplatesFound()
    {
        var node = A.Fake<Node>(o => o.Wrapping(new Node(NodeType.Document)));
        node.AddAttribute("test", "value");
        var templateNode1 = new Node(NodeType.Template);
        templateNode1.AddAttribute("test", "value1");
        A.CallTo(() => node.GetApplicableTemplateNodes()).Returns(Array.Empty<Node>());
        nodeConstructor.ApplyTemplates(node);
        Assert.That(node.Attributes, Has.Count.EqualTo(1));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("test"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("value"));
    }
    
    [Test]
    public void DifferentTemplateFound()
    {
        var node = A.Fake<Node>(o => o.CallsBaseMethods());
        node.AddAttribute("template", "notfound");
        var templateNode1 = new Node(NodeType.Template);
        templateNode1.AddAttribute("name", "template1");
        A.CallTo(() => node.GetApplicableTemplateNodes()).Returns(new[] { templateNode1 });
        nodeConstructor.ApplyTemplates(node);
        Assert.That(node.Attributes, Has.Count.EqualTo(1));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("template"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("notfound"));
    }
    
    [Test]
    public void TemplateFound()
    {
        var node = A.Fake<Node>(o => o.CallsBaseMethods());
        node.AddAttribute("template", "template1");
        var templateNode1 = new Node(NodeType.Template);
        templateNode1.AddAttribute("name", "template1");
        templateNode1.AddAttribute("test", "fromTemplate");
        A.CallTo(() => node.GetApplicableTemplateNodes()).Returns(new[] { templateNode1 });
        Assert.That(node.Attributes, Has.Count.EqualTo(1));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("template"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("template1"));
        nodeConstructor.ApplyTemplates(node);
        Assert.That(node.Attributes, Has.Count.EqualTo(2));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("template"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("template1"));
        Assert.That(node.Attributes.ElementAt(1).Key, Is.EqualTo("test"));
        Assert.That(node.Attributes.ElementAt(1).Value, Is.EqualTo("fromTemplate"));
    }
    
    [Test]
    public void MultipleTemplatesFound()
    {
        var node = A.Fake<Node>(o => o.CallsBaseMethods());
        node.AddAttribute("template", "template1 template2");
        var templateNode1 = new Node(NodeType.Template);
        templateNode1.AddAttribute("name", "template1");
        templateNode1.AddAttribute("test", "fromTemplate1");
        var templateNode2 = new Node(NodeType.Template);
        templateNode2.AddAttribute("name", "template2");
        templateNode2.AddAttribute("test2", "fromTemplate2");
        A.CallTo(() => node.GetApplicableTemplateNodes()).Returns(new[] { templateNode1, templateNode2 });
        Assert.That(node.Attributes, Has.Count.EqualTo(1));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("template"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("template1 template2"));
        
        nodeConstructor.ApplyTemplates(node);
        
        Assert.That(node.Attributes, Has.Count.EqualTo(3));
        Assert.That(node.Attributes.First().Key, Is.EqualTo("template"));
        Assert.That(node.Attributes.First().Value, Is.EqualTo("template1 template2"));
        Assert.That(node.Attributes.ElementAt(1).Key, Is.EqualTo("test"));
        Assert.That(node.Attributes.ElementAt(1).Value, Is.EqualTo("fromTemplate1"));
        Assert.That(node.Attributes.ElementAt(2).Key, Is.EqualTo("test2"));
        Assert.That(node.Attributes.ElementAt(2).Value, Is.EqualTo("fromTemplate2"));
    }
}