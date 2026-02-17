using FinalProject.World;

namespace FinalProject.Rules;

public class NamePatternRule : RuleBase
{
    public string Pattern { get; }
    public NameMatchType MatchType { get; }
    public string Destination { get; }

    public NamePatternRule(string pattern, NameMatchType matchType, string destination)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("Pattern cannot be empty.", nameof(pattern));
        }
        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("Destination cannot be empty.", nameof(destination));
        }

        Pattern = pattern;
        MatchType = matchType;
        Destination = destination;
    }

    public override bool IsMatch(VirtualFileItem item)
    {
        return MatchType switch
        {
            NameMatchType.Contains => item.Name.IndexOf(Pattern, StringComparison.OrdinalIgnoreCase) >= 0,
            NameMatchType.StartsWith => item.Name.StartsWith(Pattern, StringComparison.OrdinalIgnoreCase),
            NameMatchType.EndsWith => item.Name.EndsWith(Pattern, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    public override string DestinationName(VirtualFileItem item)
    {
        return Destination;
    }

    public override string Describe()
    {
        return $"NamePatternRule: {MatchType} '{Pattern}' -> {Destination}";
    }
}
