using FinalProject.Rules;
using FinalProject.Undo;
using FinalProject.World;

namespace FinalProject.Core;

public class GameState
{
    private const int CorrectSortScore = 10;
    private const int IncorrectSortScore = -5;
    private const int StandardActionMoveCost = 1;
    private const int IncorrectSortExtraMoveCost = 1;
    private const int UndoActionMoveCost = 1;

    public int Seed { get; }
    public Difficulty Difficulty { get; }
    public int MoveLimit { get; }
    public int MovesUsed { get; private set; }
    public int Score { get; private set; }
    public int WrongSorts { get; private set; }

    public VirtualFileSystem Vfs { get; }
    public RulePack RulePack { get; }
    public UndoStack UndoStack { get; }

    private readonly IReadOnlyDictionary<Guid, string> _truthTable;

    public GameState(int seed, Difficulty difficulty, int moveLimit, VirtualFileSystem vfs, RulePack rulePack, IReadOnlyDictionary<Guid, string> truthTable)
    {
        if (moveLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moveLimit), "Move limit must be positive.");
        }

        Seed = seed;
        Difficulty = difficulty;
        MoveLimit = moveLimit;
        Vfs = vfs ?? throw new ArgumentNullException(nameof(vfs));
        RulePack = rulePack ?? throw new ArgumentNullException(nameof(rulePack));
        _truthTable = new Dictionary<Guid, string>(truthTable ?? throw new ArgumentNullException(nameof(truthTable)));
        UndoStack = new UndoStack();
    }

    public bool InboxEmpty => Vfs.GetFolderItems("Inbox").Count == 0;

    public int InboxCount => Vfs.GetFolderItems("Inbox").Count;

    public void SpendMove()
    {
        MovesUsed += StandardActionMoveCost;
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

        int scoreDelta = isCorrect ? CorrectSortScore : IncorrectSortScore;
        int appliedScoreDelta = ApplyScoreDelta(scoreDelta);

        int moveCost = StandardActionMoveCost;
        int wrongSortDelta = 0;
        if (!isCorrect)
        {
            moveCost += IncorrectSortExtraMoveCost;
            wrongSortDelta = 1;
            WrongSorts++;
        }

        MovesUsed += moveCost;
        UndoStack.Push(new MoveRecord(item, "Inbox", destination, appliedScoreDelta, wrongSortDelta));

        return new SortResult(
            rule.Describe(),
            destination,
            correctDestination,
            isCorrect,
            moveCost);
    }

    public bool ApplyUndo()
    {
        MovesUsed += UndoActionMoveCost;

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
