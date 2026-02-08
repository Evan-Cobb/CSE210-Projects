namespace FinalProject.Domain;

public sealed class RuleDefinition
{
    public string Type { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;
    public int Priority { get; init; } = 0;
    public string Destination { get; init; } = string.Empty;
    public ActionKind Action { get; init; } = ActionKind.MoveFile;

    public List<string> Extensions { get; init; } = new List<string>();

    public string Pattern { get; init; } = string.Empty;
    public PatternMatchType Match { get; init; } = PatternMatchType.Contains;

    public DateBucket Bucket { get; init; } = DateBucket.Year;
    public DateSource DateSource { get; init; } = DateSource.Modified;
}
