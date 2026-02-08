namespace FinalProject.Domain;

public sealed class RulePack
{
    public int Version { get; init; } = 1;
    public List<string> BlockedDirectories { get; init; } = new List<string>();
    public List<string> IncludeExtensions { get; init; } = new List<string>();
    public List<string> ExcludeExtensions { get; init; } = new List<string>();
    public List<RuleDefinition> Rules { get; init; } = new List<RuleDefinition>();
}
