using FinalProject.Core;
using FinalProject.Rules;
using FinalProject.World;

namespace FinalProject.Util;

public static class ConsoleUi
{
    public static void PrintTitle()
    {
        Console.WriteLine("Virtual Organizer Move Challenge");
        Console.WriteLine("--------------------------------");
    }

    public static void PrintRules(RulePack pack)
    {
        Console.WriteLine("Current Rules (priority order):");
        if (pack.Rules.Count == 0)
        {
            Console.WriteLine("  (none)");
            return;
        }
        for (int i = 0; i < pack.Rules.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {pack.Rules[i].Describe()}");
        }
    }

    public static void PrintInbox(IReadOnlyList<VirtualFileItem> items)
    {
        Console.WriteLine("Inbox:");
        if (items.Count == 0)
        {
            Console.WriteLine("  (empty)");
            return;
        }
        for (int i = 0; i < items.Count; i++)
        {
            VirtualFileItem item = items[i];
            Console.WriteLine($"  {i + 1}. {item.Name} | Created {item.CreatedUtc:yyyy-MM-dd} | Modified {item.ModifiedUtc:yyyy-MM-dd}");
        }
    }

    public static void PrintSummary(GameState state, bool won, bool quit)
    {
        Console.WriteLine();
        Console.WriteLine("=== Summary ===");
        Console.WriteLine($"Seed: {state.Seed}");
        Console.WriteLine($"Difficulty: {state.Difficulty}");
        Console.WriteLine($"Moves: {state.MovesUsed}/{state.MoveLimit}");
        Console.WriteLine($"Score: {state.Score}");
        Console.WriteLine($"Wrong sorts: {state.WrongSorts}");

        if (quit)
        {
            Console.WriteLine("Outcome: Quit");
            return;
        }

        Console.WriteLine($"Outcome: {(won ? "Win" : "Loss")}");

        if (!won)
        {
            IReadOnlyList<VirtualFileItem> remaining = state.Vfs.GetFolderItems("Inbox");
            Console.WriteLine($"Remaining items: {remaining.Count}");
            for (int i = 0; i < remaining.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {remaining[i].Name}");
            }
        }
    }
}
