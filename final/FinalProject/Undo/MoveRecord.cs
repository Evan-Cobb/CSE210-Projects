using FinalProject.World;

namespace FinalProject.Undo;

public class MoveRecord
{
    public VirtualFileItem Item { get; }
    public string FromFolder { get; }
    public string ToFolder { get; }
    public bool WasCorrect { get; }
    public int ScoreDelta { get; }
    public int WrongSortDelta { get; }

    public MoveRecord(VirtualFileItem item, string fromFolder, string toFolder, bool wasCorrect, int scoreDelta, int wrongSortDelta)
    {
        Item = item;
        FromFolder = fromFolder;
        ToFolder = toFolder;
        WasCorrect = wasCorrect;
        ScoreDelta = scoreDelta;
        WrongSortDelta = wrongSortDelta;
    }
}
