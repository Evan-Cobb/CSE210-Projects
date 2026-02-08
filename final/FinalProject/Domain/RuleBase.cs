namespace FinalProject.Domain;

public abstract class RuleBase : IFileRule
{
    public string Name { get; }
    public int Priority { get; }
    public bool Enabled { get; }
    public string Destination { get; }
    public ActionKind ActionKind { get; }

    protected RuleBase(string name, int priority, bool enabled, string destination, ActionKind actionKind)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "UnnamedRule" : name.Trim();
        Priority = priority;
        Enabled = enabled;
        Destination = destination ?? string.Empty;
        ActionKind = actionKind;
    }

    public bool TryMatch(FileItem item, out RuleMatch match)
    {
        match = null;

        if (!Enabled)
        {
            return false;
        }

        if (!IsMatch(item, out string destinationSubPath))
        {
            return false;
        }

        match = new RuleMatch(Name, ActionKind, destinationSubPath);
        return true;
    }

    protected abstract bool IsMatch(FileItem item, out string destinationSubPath);
}
