using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.Nodes;

namespace Lattice.AttributeMutators;

[Export<TextStyleMutator>]
public class TextStyleMutator(ColourConverter ColourConverter, ContextReplacer ContextReplacer)
{
    public TextStyle Mutate(TextStyle textStyle, Node node)
    {
        var style = textStyle;
        
        var lineHeight = node.GetAttributeFloat("lineHeight");
        if (lineHeight is not null)
        {
            style = style.LineHeight(lineHeight.Value);
        }
        
        var letterSpacing  = node.GetAttributeFloat("letterSpacing");
        if (letterSpacing is not null)
        {
            style = style.LetterSpacing(letterSpacing.Value);
        }
        
        
        var fontFamily = node.GetAttribute("fontFamily");
        if (fontFamily is not null)
        {
            style = style.FontFamily(fontFamily);
        }
        
        var fontColour = node.GetAttribute("fontColour");
        if (fontColour is not null)
        {
            fontColour = ContextReplacer.ReplaceTokens(fontColour, node);
            style = style.FontColor(ColourConverter.ConvertToHex(fontColour));
        }
        
        var fontSize = node.GetAttributeFloat("fontSize");
        if (fontSize is not null)
        {
            style = style.FontSize(fontSize.Value);
        }
        
        var emphasis = (node.GetAttribute("fontEmphasis") ?? "").Split(" ");
       
        foreach (var emp in emphasis)
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