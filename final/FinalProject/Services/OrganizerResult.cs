using FinalProject.Domain;

namespace FinalProject.Services;

public sealed class OrganizerResult
{
    public RunLog RunLog { get; }
    public int Succeeded { get; }
    public int Failed { get; }
    public int Skipped { get; }
    public List<string> Warnings { get; }

    public OrganizerResult(RunLog runLog, int succeeded, int failed, int skipped, List<string> warnings)
    {
        RunLog = runLog;
        Succeeded = succeeded;
        Failed = failed;
        Skipped = skipped;
        Warnings = warnings ?? new List<string>();
    }
}
