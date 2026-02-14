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

    private readonly Dictionary<Guid, string> _truthTable;

    public GameState(int seed, Difficulty difficulty, int turnLimit, VirtualFileSystem vfs, RulePack rulePack, Dictionary<Guid, string> truthTable)
    {
        Seed = seed;
        Difficulty = difficulty;
        TurnLimit = turnLimit;
        Vfs = vfs;
        RulePack = rulePack;
        _truthTable = truthTable;
        UndoStack = new UndoStack();
    }

    public bool InboxEmpty => Vfs.GetFolderItems("Inbox").Count == 0;

    public int InboxCount => Vfs.GetFolderItems("Inbox").Count;

    public string GetCorrectFolder(VirtualFileItem item)
    {
        return _truthTable[item.Id];
    }

    public void AddTurns(int amount)
    {
        TurnsUsed += amount;
    }

    public int ApplyScoreDelta(int delta)
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

    public void AddWrongSorts(int amount)
    {
        int value = WrongSorts + amount;
        if (value < 0)
        {
            value = 0;
        }
        WrongSorts = value;
    }
}
