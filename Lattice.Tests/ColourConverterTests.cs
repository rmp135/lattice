namespace Lattice.Tests;
using Lattice;

public class ColourConverterTests
{
    private readonly ColourConverter _colourConverter = new();

    [Test]
    public void KnownColour()
    {
        var outColour = _colourConverter.ConvertToHex("rEd");
        Assert.That(outColour, Is.EqualTo("#FF0000"));
        Assert.Pass();
    }

    [Test]
    public void UnknownColour()
    {
        var outColour = _colourConverter.ConvertToHex("blag");
        Assert.That(outColour, Is.EqualTo("blag"));
        Assert.Pass();
    }
}