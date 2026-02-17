using FinalProject.Core;
using FinalProject.Util;
using FinalProject.World;

namespace FinalProject.Game;

public class CombatEngine
{
    public void Run(GameState state)
    {
        bool quit = false;

        while (true)
        {
            if (state.InboxEmpty && state.TurnsUsed <= state.TurnLimit)
            {
                Console.WriteLine("Inbox cleared!");
                break;
            }
            if (state.TurnsUsed > state.TurnLimit)
            {
                Console.WriteLine("Turn limit exceeded.");
                break;
            }

            Console.WriteLine();
            Console.WriteLine($"Turns: {state.TurnsUsed}/{state.TurnLimit} | Score: {state.Score} | Inbox: {state.InboxCount}");
            Console.WriteLine("Actions:");
            Console.WriteLine("1) Sort");
            Console.WriteLine("2) Undo");
            Console.WriteLine("3) View Inbox");
            Console.WriteLine("4) View Rules");
            Console.WriteLine("5) Quit");

            int choice = Input.ReadIntInRange("Choice: ", 1, 5);
            Console.WriteLine();

            if (choice == 1)
            {
                Sort(state);
            }
            else if (choice == 2)
            {
                Undo(state);
            }
            else if (choice == 3)
            {
                state.SpendTurn();
                ConsoleUi.PrintInbox(state.Vfs.GetFolderItems("Inbox"));
            }
            else if (choice == 4)
            {
                state.SpendTurn();
                ConsoleUi.PrintRules(state.RulePack);
            }
            else
            {
                state.SpendTurn();
                quit = true;
                break;
            }
        }

        bool won = state.InboxEmpty && state.TurnsUsed <= state.TurnLimit;
        ConsoleUi.PrintSummary(state, won, quit);
    }

    private static void Sort(GameState state)
    {
        if (state.InboxEmpty)
        {
            Console.WriteLine("Inbox is empty.");
            state.SpendTurn();
            return;
        }

        IReadOnlyList<VirtualFileItem> inbox = state.Vfs.GetFolderItems("Inbox");
        ConsoleUi.PrintInbox(inbox);

        int index = Input.ReadIntInRange("Pick item: ", 1, inbox.Count);
        VirtualFileItem item = inbox[index - 1];
        SortResult result = state.ApplySort(item);

        Console.WriteLine($"Rule used: {result.RuleDescription}");
        Console.WriteLine($"Moved to: {result.Destination}");
        if (result.IsCorrect)
        {
            Console.WriteLine("Result: Correct");
        }
        else
        {
            Console.WriteLine($"Result: Incorrect (correct: {result.CorrectDestination})");
        }
    }

    private static void Undo(GameState state)
    {
        bool undone = state.ApplyUndo();
        if (!undone)
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        Console.WriteLine("Undo applied.");
    }
}
