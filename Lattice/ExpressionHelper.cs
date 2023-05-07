using System.Text.RegularExpressions;
using Lattice.Nodes;
using org.matheval;

namespace Lattice;

[Export(typeof(ExpressionHelper))]
public class ExpressionHelper
{
    public string RunExpression(Node node, string token)
    {
        var contextItems = node.GetAllContextValues();
        var modifiedDict = new Dictionary<string, object>();
        var regex = new Regex("[^a-zA-Z]");

        foreach (var kvp in contextItems)
        {
            var modifiedKey = regex.Replace(kvp.Key, "");
            if (!token.Contains(kvp.Key)) continue;
            token = token.Replace(kvp.Key, modifiedKey);
            if (decimal.TryParse(kvp.Value.ToString(), out var value))
            {
                modifiedDict[modifiedKey] = value;
            }
            else
            {
                modifiedDict[modifiedKey] = kvp.Value.ToString() ?? "";
            }
        }
        var expression = new Expression(token);
        foreach (var contextItem in modifiedDict)
        {
            expression.Bind(contextItem.Key, contextItem.Value);
        }

        try
        {
            return expression.Eval()?.ToString() ?? "";
        }
        catch (Exception e)
        {
            return token;
        }
    }
}