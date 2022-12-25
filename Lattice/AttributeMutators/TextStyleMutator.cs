using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;

[Export(typeof(TextStyleMutator))]
public class TextStyleMutator
{
    private readonly ColourConverter ColourConverter;
    private readonly ContextReplacer ContextReplacer;

    public TextStyleMutator(ColourConverter colourConverter, ContextReplacer contextReplacer)
    {
        ColourConverter = colourConverter;
        ContextReplacer = contextReplacer;
    }

    public TextStyle Mutate(TextStyle textStyle, Node node)
    {
        var fontSize = node.GetAttributeInt("fontSize");
        var fontColour = node.GetAttribute("fontColour");
        var fontFamily = node.GetAttribute("fontFamily");
        var style = textStyle;
        if (fontFamily is not null)
        {
            style = style.FontFamily(fontFamily);
        }
        if (fontColour is not null)
        {
            fontColour = ContextReplacer.ReplaceTokens(fontColour, node);
            style = style.FontColor(ColourConverter.ConvertToHex(fontColour));
        }
        if (fontSize is not null)
        {
            style = style.FontSize(fontSize.Value);
        }
        
        var emphasis = (node.GetAttribute("fontEmphasis") ?? "").Split(" ");
       
        foreach (var emp  in emphasis)
        {
            style = emp switch
            {
                "italic" => style.Italic(),
                "thin" => style.Thin(),
                "extraLight" => style.ExtraLight(),
                "light" => style.Light(),
                "medium" => style.Medium(),
                "semiBold" => style.SemiBold(),
                "bold" => style.Bold(),
                "extraBold" => style.ExtraBold(),
                "black" => style.Black(),
                "extraBlack" => style.ExtraBlack(),
                "underline" => style.Underline(),
                "strikethrough" => style.Strikethrough(),
                "superscript" => style.Superscript(),
                "subscript" => style.Subscript(),
                _ => style
            };
        }
        
        return style;
    }
}