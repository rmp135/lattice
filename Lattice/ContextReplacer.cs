using Lattice.Nodes;
using System.Text;

namespace Lattice;

[Export(typeof(ContextReplacer))]
public class ContextReplacer
{
    public string ReplaceTokens(string text, Node node)
    {
        var tokens = GetTokens(text);
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            if (token.IsToken)
            {
                builder.Append(node.GetContextValue(token.Text)?.ToString() ?? token.Text);
            }
            else
            {
                builder.Append(token.Text);
            }
        }
        return builder.ToString();
    }

    public IEnumerable<TokenResult> GetTokens(string text)
    {
        var currentText = new StringBuilder();
        var tokens = new List<TokenResult>();
        foreach (var t in text)
        {
            if (t == '{')
            {
                tokens.Add(new TokenResult(currentText.ToString(), false));
                currentText.Clear();
            }
            else if (t == '}')
            {
                tokens.Add(new TokenResult(currentText.ToString(), true));
                currentText.Clear();
            }
            else
            {
                currentText.Append(t);
            }
        }
        tokens.Add(new TokenResult(currentText.ToString(), false));
        return tokens.Where(t => !string.IsNullOrEmpty(t.Text));
    }
}

public class TokenResult
{
    public TokenResult(string text, bool isToken)
    {
        Text = text;
        IsToken = isToken;
    }

    public bool IsToken { get; }
    public string Text { get; }
}