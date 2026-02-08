using FinalProject.Domain;

namespace FinalProject.Rules;

public sealed class FallbackRule : RuleBase
{
    public FallbackRule(string name, int priority, bool enabled, string destination, ActionKind actionKind)
        : base(name, priority, enabled, destination, actionKind)
    {
    }

    protected override bool IsMatch(FileItem item, out string destinationSubPath)
    {
        destinationSubPath = Destination;
        return true;
    }
}
