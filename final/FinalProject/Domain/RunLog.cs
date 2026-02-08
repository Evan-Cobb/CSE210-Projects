namespace FinalProject.Domain;

public sealed class RunLog
{
    public Guid RunId { get; init; } = Guid.NewGuid();
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
    public string RootPath { get; init; } = string.Empty;
    public ConflictPolicy ConflictPolicy { get; init; } = ConflictPolicy.RenameWithCounter;
    public List<RunLogEntry> Entries { get; init; } = new List<RunLogEntry>();
}

public sealed class RunLogEntry
{
    public ActionKind ActionKind { get; init; } = ActionKind.MoveFile;
    public string SourcePath { get; init; } = string.Empty;
    public string DestinationPath { get; init; } = string.Empty;
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}
