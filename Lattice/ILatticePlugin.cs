using Lattice.Nodes;
using QuestPDF.Infrastructure;

namespace Lattice;

public interface ILatticePlugin
{
    public string Tag { get; }

    void Build(
        Node node,
        IContainer container
    );
}