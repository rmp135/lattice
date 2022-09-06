namespace Lattice.Nodes;

public class TextNode : Node
{
    public TextNode(string text) : base(NodeType.Text)
    {
        AddAttribute("text", text);
    }
}