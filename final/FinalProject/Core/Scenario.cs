using FinalProject.World;

namespace FinalProject.Core;

public class Scenario
{
    public int Seed { get; }
    public Difficulty Difficulty { get; }
    public int TurnLimit { get; }
    public VirtualFileSystem Vfs { get; }
    public IReadOnlyDictionary<Guid, string> TruthTable { get; }
    public IReadOnlyList<VirtualFileItem> Items { get; }

    public Scenario(int seed, Difficulty difficulty, int turnLimit, VirtualFileSystem vfs, IReadOnlyDictionary<Guid, string> truthTable, IReadOnlyList<VirtualFileItem> items)
    {
        if (turnLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(turnLimit), "Turn limit must be positive.");
        }

        Seed = seed;
        Difficulty = difficulty;
        TurnLimit = turnLimit;
        Vfs = vfs ?? throw new ArgumentNullException(nameof(vfs));
        TruthTable = new Dictionary<Guid, string>(truthTable ?? throw new ArgumentNullException(nameof(truthTable)));
        Items = new List<VirtualFileItem>(items ?? throw new ArgumentNullException(nameof(items))).AsReadOnly();
    }
}
