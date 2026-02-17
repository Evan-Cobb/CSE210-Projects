using FinalProject.Rules;
using FinalProject.Undo;
using FinalProject.World;

namespace FinalProject.Core;

public class GameState
{
    public int Seed { get; }
    public Difficulty Difficulty { get; }
    public int TurnLimit { get; }
    public int TurnsUsed { get; private set; }
    public int Score { get; private set; }
    public int WrongSorts { get; private set; }

    public VirtualFileSystem Vfs { get; }
    public RulePack RulePack { get; }
    public UndoStack UndoStack { get; }

    private readonly IReadOnlyDictionary<Guid, string> _truthTable;

    public GameState(int seed, Difficulty difficulty, int turnLimit, VirtualFileSystem vfs, RulePack rulePack, IReadOnlyDictionary<Guid, string> truthTable)
    {
        if (turnLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(turnLimit), "Turn limit must be positive.");
        }

        Seed = seed;
        Difficulty = difficulty;
        TurnLimit = turnLimit;
        Vfs = vfs ?? throw new ArgumentNullException(nameof(vfs));
        RulePack = rulePack ?? throw new ArgumentNullException(nameof(rulePack));
        _truthTable = new Dictionary<Guid, string>(truthTable ?? throw new ArgumentNullException(nameof(truthTable)));
        UndoStack = new UndoStack();
    }

    public bool InboxEmpty => Vfs.GetFolderItems("Inbox").Count == 0;

    public int InboxCount => Vfs.GetFolderItems("Inbox").Count;

    public void SpendTurn()
    {
        TurnsUsed++;
    }

    public SortResult ApplySort(VirtualFileItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        RuleBase rule = RulePack.Pick(item);
        string destination = rule.DestinationName(item);
        string correctDestination = _truthTable[item.Id];
        bool isCorrect = string.Equals(destination, correctDestination, StringComparison.OrdinalIgnoreCase);

        Vfs.MoveItem(item, "Inbox", destination);

        int scoreDelta = isCorrect ? 10 : -5;
        int appliedScoreDelta = ApplyScoreDelta(scoreDelta);

        int turnCost = 1;
        int wrongSortDelta = 0;
        if (!isCorrect)
        {
            turnCost++;
            wrongSortDelta = 1;
            WrongSorts++;
        }

        TurnsUsed += turnCost;
        UndoStack.Push(new MoveRecord(item, "Inbox", destination, appliedScoreDelta, wrongSortDelta));

        return new SortResult(
            rule.Describe(),
            destination,
            correctDestination,
            isCorrect,
            turnCost);
    }

    public bool ApplyUndo()
    {
        TurnsUsed++;

        if (!UndoStack.TryPop(out MoveRecord record))
        {
            return false;
        }

        Vfs.MoveItem(record.Item, record.ToFolder, record.FromFolder);
        ApplyScoreDelta(-record.ScoreDelta);
        AddWrongSorts(-record.WrongSortDelta);
        return true;
    }

    private int ApplyScoreDelta(int delta)
    {
        int oldScore = Score;
        int newScore = oldScore + delta;
        if (newScore < 0)
        {
            newScore = 0;
        }
        Score = newScore;
        return newScore - oldScore;
    }

    private void AddWrongSorts(int amount)
    {
        int value = WrongSorts + amount;
        if (value < 0)
        {
            value = 0;
        }
        WrongSorts = value;
    }
}
