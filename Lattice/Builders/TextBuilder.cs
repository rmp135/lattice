using System.Globalization;
using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Lattice.AttributeMutators;
using Lattice.Nodes;

namespace Lattice.Builders;

[Export(typeof(IContainerBuilder))]
[AutoConstructor]
public partial class TextBuilder : IContainerBuilder
{
    private readonly TextStyleMutator StyleMutator;
    private readonly ContextReplacer ContextReplacer;
    private readonly ContainerMutator ContainerMutator;
    private readonly IEnumerable<IAggregator> Aggregators;

    public NodeType Type => NodeType.Text;

    public void Build(Node node, IContainer container)
    {
        var value = node.GetAttribute("text");
        
        var newContainer = ContainerMutator.Mutate(container, node);
        if (string.IsNullOrEmpty(value)) return;
        newContainer.Text(textDescriptor =>
        {
            textDescriptor.DefaultTextStyle(s => StyleMutator.Mutate(s, node));
            var align = node.GetAttribute("align");
            if (align == "center")
                textDescriptor.AlignCenter();
            if (align == "left")
                textDescriptor.AlignLeft();
            if (align == "right")
                textDescriptor.AlignRight();

            var text = !string.IsNullOrEmpty(value) ? value : "No text given.";
            var tokens = ContextReplacer.GetTokens(text);

            foreach (var token in tokens)
            {
                var val = token.Text;
                if (token.IsToken)
                {
                    if (string.Equals(token.Text, "totalpages", StringComparison.InvariantCultureIgnoreCase))
                    {
                        textDescriptor.TotalPages();
                        continue;
                    }
                    if (string.Equals(token.Text, "currentpage", StringComparison.InvariantCultureIgnoreCase))
                    {
                        textDescriptor.CurrentPageNumber();
                        continue;
                    }

                    foreach (var aggregator in Aggregators)
                    {
                        val = aggregator.Aggregate(node, val);
                    }

                    val = ContextReplacer.ReplaceToken(node, token.Text);
                    textDescriptor.Span(val);
                }
                else
                {
                    textDescriptor.Span(token.Text);
                }
            }
        });
    }
}

public interface IAggregator
{
    string Aggregate(Node node, string value);
}

public abstract class BaseAggregator : IAggregator
{
    protected abstract Regex Regex { get; }

    public string Aggregate(Node node, string value)
    {
        var matches = Regex.Matches(value);
        if (!Regex.Matches(value).Any())
        {
            return value;
        }

        var groups = matches.First().Groups;
        var setKey = groups[1].Value;
        var propertyKey = groups[2].Value;
        var set = node.GetContextValue(setKey);
        if (set is null)
        {
            return value;
        }
        var allValues = set
            .SelectMany(d => d)
            .Where(d => d.Key.Equals(propertyKey));
        return set?.FirstOrDefault()?.ContainsKey(propertyKey) != true ? value : Aggregate(allValues, groups);
    }

    protected abstract string Aggregate(IEnumerable<KeyValuePair<string, object>> set, GroupCollection groups);
}

[Export(typeof(IAggregator))]
public class SumAggregator: BaseAggregator
{
    protected override Regex Regex { get; } =  new Regex(@"SUM\((\w+)\.(\w+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    protected override string Aggregate(IEnumerable<KeyValuePair<string, object>> set, GroupCollection groups)
    {
        return set
            .Sum(s => s.Value switch
            {
                int asInt => asInt,
                decimal asDecimal => asDecimal,
                _ => 0
            }).ToString(CultureInfo.InvariantCulture);
    }
}

[Export(typeof(IAggregator))]
public class AvgAggregator: BaseAggregator
{
    protected override Regex Regex { get; } = new Regex(@"(?:AVG|AVERAGE)\((\w+)\.(\w+)(?:,(\W*[0-9]+))?\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    protected override string Aggregate(IEnumerable<KeyValuePair<string, object>> set, GroupCollection groups)
    {
        var total = set
            .Average(s => s.Value switch
            {
                int asInt => asInt,
                decimal asDecimal => asDecimal,
                _ => 0
            });
        // Rounding can be specified.
        if (groups.Count == 4 && !string.IsNullOrEmpty(groups[3].Value))
        {
            var format = $"0.{Enumerable.Range(0, int.Parse(groups[3].Value)).Select(_ => "#").Aggregate((a, b) => $"{a}{b}")}";
            return total.ToString(format);
        }
        return total.ToString(CultureInfo.InvariantCulture);
    }
}


[Export(typeof(IAggregator))]
public class MinAggregator: BaseAggregator
{
    protected override Regex Regex { get; } = new Regex(@"MIN\((\w+)\.(\w+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    protected override string Aggregate(IEnumerable<KeyValuePair<string, object>> set, GroupCollection groups)
    {
        return set
            .MinBy(s => s.Value switch
            {
                int asInt => asInt,
                decimal asDecimal => asDecimal,
                DateTime asDateTime => asDateTime.Ticks,
                _ => 0
            }).Value.ToString()!;
    }
}

[Export(typeof(IAggregator))]
public class MaxAggregator: BaseAggregator
{
    protected override Regex Regex { get; } = new Regex(@"MAX\((\w+)\.(\w+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    protected override string Aggregate(IEnumerable<KeyValuePair<string, object>> set, GroupCollection groups)
    {
        return set
            .MaxBy(s => s.Value switch
            {
                int asInt => asInt,
                decimal asDecimal => asDecimal,
                DateTime asDateTime => asDateTime.Ticks,
                _ => 0
            }).Value.ToString()!;
    }
}

[Export(typeof(IAggregator))]
public class CountAggregator: IAggregator
{
    private readonly Regex regex = new Regex(@"COUNT\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public string Aggregate(Node node, string value)
    {
        var matches = regex.Matches(value);
        if (!regex.Matches(value).Any())
        {
            return value;
        }

        var groups = matches.First().Groups;
        var setKey = groups[1].Value;
        var set = node.GetContextValue(setKey);
        return set?.Count().ToString() ?? value;
    }
}