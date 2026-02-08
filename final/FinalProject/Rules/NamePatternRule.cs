using FinalProject.Domain;

namespace FinalProject.Rules;

public sealed class NamePatternRule : RuleBase
{
    private readonly string _pattern;
    private readonly PatternMatchType _matchType;

    public NamePatternRule(string name, int priority, bool enabled, string destination, ActionKind actionKind, string pattern, PatternMatchType matchType)
        : base(name, priority, enabled, destination, actionKind)
    {
        _pattern = pattern ?? string.Empty;
        _matchType = matchType;
    }

    protected override bool IsMatch(FileItem item, out string destinationSubPath)
    {
        destinationSubPath = Destination;

        if (string.IsNullOrWhiteSpace(_pattern))
        {
            return false;
        }

        string target = item.FileNameWithoutExtension;
        StringComparison comparison = StringComparison.OrdinalIgnoreCase;

        return _matchType switch
        {
            PatternMatchType.StartsWith => target.StartsWith(_pattern, comparison),
            PatternMatchType.EndsWith => target.EndsWith(_pattern, comparison),
            _ => target.IndexOf(_pattern, comparison) >= 0
        };
    }
}
