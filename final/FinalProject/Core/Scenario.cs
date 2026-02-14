using FinalProject.World;

namespace FinalProject.Core;

public class Scenario
{
    public int Seed { get; }
    public Difficulty Difficulty { get; }
    public int TurnLimit { get; }
    public VirtualFileSystem Vfs { get; }
    public Dictionary<Guid, string> TruthTable { get; }
    public IReadOnlyList<VirtualFileItem> Items { get; }

    public Scenario(int seed, Difficulty difficulty, int turnLimit, VirtualFileSystem vfs, Dictionary<Guid, string> truthTable, IReadOnlyList<VirtualFileItem> items)
    {
        Seed = seed;
        Difficulty = difficulty;
        TurnLimit = turnLimit;
        Vfs = vfs;
        TruthTable = truthTable;
        Items = items;
    }
}
