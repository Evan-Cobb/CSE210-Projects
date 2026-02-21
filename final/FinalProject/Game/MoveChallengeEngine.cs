using FinalProject.Core;
using FinalProject.Util;
using FinalProject.World;

namespace FinalProject.Game;

public class MoveChallengeEngine
{
    public void Run(GameState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        bool quit = false;

        while (true)
        {
            if (state.InboxEmpty && state.MovesUsed <= state.MoveLimit)
            {
                Console.WriteLine("Inbox cleared!");
                break;
            }
            if (state.MovesUsed > state.MoveLimit)
            {
                Console.WriteLine("Move limit exceeded.");
                break;
            }

            Console.WriteLine();
            Console.WriteLine($"Moves: {state.MovesUsed}/{state.MoveLimit} | Score: {state.Score} | Inbox: {state.InboxCount}");
            Console.WriteLine("Actions:");
            Console.WriteLine("1) Sort");
            Console.WriteLine("2) Undo");
            Console.WriteLine("3) View Inbox (-1 move)");
            Console.WriteLine("4) View Rules (-1 move)");
            Console.WriteLine("5) Add Rule (-1 move)");
            Console.WriteLine("6) Change Rule Priority (-1 move)");
            Console.WriteLine("7) Quit (-1 move)");

            int choice = Input.ReadIntInRange("Choice: ", 1, 7);
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
                state.SpendMove();
                ConsoleUi.PrintInbox(state.Vfs.GetFolderItems("Inbox"));
            }
            else if (choice == 4)
            {
                state.SpendMove();
                ConsoleUi.PrintRules(state.RulePack);
            }
            else if (choice == 5)
            {
                state.SpendMove();
                RuleWorkshop.AddRuleDuringChallenge(state.RulePack, state.Vfs.FolderNames);
            }
            else if (choice == 6)
            {
                state.SpendMove();
                RuleWorkshop.ReorderDuringChallenge(state.RulePack);
            }
            else
            {
                state.SpendMove();
                quit = true;
                break;
            }
        }

        bool won = state.InboxEmpty && state.MovesUsed <= state.MoveLimit;
        ConsoleUi.PrintSummary(state, won, quit);
    }

    private static void Sort(GameState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (state.InboxEmpty)
        {
            Console.WriteLine("Inbox is empty.");
            state.SpendMove();
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
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        bool undone = state.ApplyUndo();
        if (!undone)
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        Console.WriteLine("Undo applied.");
    }
}
