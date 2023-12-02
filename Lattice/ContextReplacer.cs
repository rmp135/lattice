using Lattice.Nodes;
using System.Text;

namespace Lattice;

/// <summary>
/// Methods concerning replacing the tokens from context.
/// </summary>
[Export(typeof(ContextReplacer))]
[AutoConstructor]
public partial class ContextReplacer
{
    private readonly ExpressionHelper ExpressionHelper;

    /// <summary>
    /// Replaces all tokens in a given text string, given the context of a Node.
    /// </summary>
    /// <param name="text">The text string to replace tokens for.</param>
    /// <param name="node">The <see cref="Node"/> context to use.</param>
    /// <returns>The original text string with tokens replaced.</returns>
    public string ReplaceTokens(string text, Node node)
    {
        var tokens = GetTokens(text);
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            if (token.IsToken)
            {
                builder.Append(ReplaceToken(node, token.Text));
            }
            else
            {
                builder.Append(token.Text);
            }
        }
        return builder.ToString();
    }

    public virtual string ReplaceToken(
        Node node,
        string token
    )
    {
        var existingContextValue = node.GetContextValue(token)?.ToString();
        if (existingContextValue != null)
        {
            return existingContextValue;
        }

        
        try 
        {
            return ExpressionHelper.RunExpression(node, token);
        }
        catch (Exception)
        {
            return token;
        }
    }

    /// <summary>
    /// Returns tokens for a given text string.
    /// </summary>
    /// <param name="text">The string to extract tokens from.</param>
    /// <returns>The extracted tokens.</returns>
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

    /// <summary>
    /// Whether the given TokenResult is actually a token.
    /// </summary>
    public bool IsToken { get; }
    /// <summary>
    /// The text content of the TokenResult.
    /// </summary>
    public string Text { get; }
}