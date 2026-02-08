namespace FinalProject.Services;

public sealed class UndoResult
{
    public int Reverted { get; }
    public int Skipped { get; }
    public int Failed { get; }
    public List<string> Warnings { get; }

    public UndoResult(int reverted, int skipped, int failed, List<string> warnings)
    {
        Reverted = reverted;
        Skipped = skipped;
        Failed = failed;
        Warnings = warnings ?? new List<string>();
    }
}
