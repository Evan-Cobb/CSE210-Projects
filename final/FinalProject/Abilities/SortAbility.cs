using FinalProject.Core;
using FinalProject.Rules;
using FinalProject.Undo;
using FinalProject.Util;
using FinalProject.World;

namespace FinalProject.Abilities;

public class SortAbility : AbilityBase
{
    public SortAbility() : base("Sort")
    {
    }

    public override void Use(GameState state)
    {
        if (state.InboxEmpty)
        {
            Console.WriteLine("Inbox is empty.");
            state.AddTurns(1);
            return;
        }

        IReadOnlyList<VirtualFileItem> inbox = state.Vfs.GetFolderItems("Inbox");
        ConsoleUi.PrintInbox(inbox);

        int index = Input.ReadIntInRange("Pick item: ", 1, inbox.Count);
        VirtualFileItem item = inbox[index - 1];

        RuleBase rule = state.RulePack.Pick(item);
        if (rule == null)
        {
            Console.WriteLine("No matching rule found.");
            state.AddTurns(1);
            return;
        }

        string destination = rule.DestinationName(item);
        string correct = state.GetCorrectFolder(item);

        bool isCorrect = string.Equals(destination, correct, StringComparison.OrdinalIgnoreCase);

        state.Vfs.MoveItem(item, "Inbox", destination);

        int scoreDelta = isCorrect ? 10 : -5;
        int appliedScoreDelta = state.ApplyScoreDelta(scoreDelta);
        int wrongSortDelta = isCorrect ? 0 : 1;
        if (!isCorrect)
        {
            state.AddTurns(1);
            state.AddWrongSorts(1);
        }

        state.AddTurns(1);

        MoveRecord record = new MoveRecord(item, "Inbox", destination, isCorrect, appliedScoreDelta, wrongSortDelta);
        state.UndoStack.Push(record);

        Console.WriteLine($"Rule used: {rule.Describe()}");
        Console.WriteLine($"Moved to: {destination}");
        Console.WriteLine(isCorrect ? "Result: Correct" : $"Result: Incorrect (correct: {correct})");
    }
}
