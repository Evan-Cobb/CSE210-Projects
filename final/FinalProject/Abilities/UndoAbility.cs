using FinalProject.Core;

namespace FinalProject.Abilities;

public class UndoAbility : AbilityBase
{
    public UndoAbility() : base("Undo")
    {
    }

    public override void Use(GameState state)
    {
        state.AddTurns(1);

        if (!state.UndoStack.TryPop(out var record))
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        state.Vfs.MoveItem(record.Item, record.ToFolder, record.FromFolder);
        state.ApplyScoreDelta(-record.ScoreDelta);
        state.AddWrongSorts(-record.WrongSortDelta);

        Console.WriteLine($"Undo: Moved '{record.Item.Name}' back to {record.FromFolder}.");
    }
}
