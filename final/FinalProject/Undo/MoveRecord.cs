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
        Item = item;
        FromFolder = fromFolder;
        ToFolder = toFolder;
        ScoreDelta = scoreDelta;
        WrongSortDelta = wrongSortDelta;
    }
}
