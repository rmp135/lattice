using System.Drawing;

namespace Lattice;

[Export(typeof(ColourConverter))]
public class ColourConverter
{
    /// <summary>
    /// Attempts to convert a given colour into a known colour.
    /// </summary>
    /// <param name="colour">Colour to attempt to convert.</param>
    /// <returns>A known colour HEX, or the original parameter if no conversion can take place.</returns>
    public string ConvertToHex(string colour)
    {
        if (!Enum.TryParse<KnownColor>(colour, true, out var knownColor)) return colour;
        var c = Color.FromKnownColor(knownColor);
        return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}