namespace FinalProject.Domain;

public sealed class RuleMatch
{
    public string RuleName { get; }
    public ActionKind ActionKind { get; }
    public string DestinationSubPath { get; }

    public RuleMatch(string ruleName, ActionKind actionKind, string destinationSubPath)
    {
        RuleName = string.IsNullOrWhiteSpace(ruleName) ? "UnnamedRule" : ruleName.Trim();
        ActionKind = actionKind;
        DestinationSubPath = destinationSubPath ?? string.Empty;
    }
}
