using System.Drawing;

namespace Lattice;

[Export(typeof(ColourConverter))]
public class ColourConverter
{
    public string ConvertToHex(string colour)
    {
        if (!Enum.TryParse<KnownColor>(colour, true, out var knownColor)) return colour;
        var c = Color.FromKnownColor(knownColor);
        return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}