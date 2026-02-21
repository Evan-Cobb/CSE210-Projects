using FinalProject.World;

namespace FinalProject.Undo;

public class MoveRecord
{
    public VirtualFileItem Item { get; }
    public string FromFolder { get; }
    public string ToFolder { get; }
    public int ScoreDelta { get; }
    public int WrongSortDelta { get; }

    public MoveRecord(VirtualFileItem item, string fromFolder, string toFolder, int scoreDelta, int wrongSortDelta)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        FromFolder = string.IsNullOrWhiteSpace(fromFolder)
            ? throw new ArgumentException("Source folder cannot be empty.", nameof(fromFolder))
            : fromFolder;
        ToFolder = string.IsNullOrWhiteSpace(toFolder)
            ? throw new ArgumentException("Destination folder cannot be empty.", nameof(toFolder))
            : toFolder;
        ScoreDelta = scoreDelta;
        WrongSortDelta = wrongSortDelta;
    }
}
