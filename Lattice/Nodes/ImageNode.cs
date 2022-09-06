namespace Lattice.Nodes;

public class ImageNode : Node
{
    public ImageNode(string source) : base(NodeType.Image)
    {
        AddAttribute("src", source);
    }
}