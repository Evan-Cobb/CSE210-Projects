using FinalProject.Abilities;
using FinalProject.Core;
using FinalProject.Util;

namespace FinalProject.Game;

public class CombatEngine
{
    private readonly AbilityBase _sort;
    private readonly AbilityBase _undo;
    private readonly AbilityBase _viewInbox;
    private readonly AbilityBase _viewRules;
    private readonly AbilityBase _addRule;
    private readonly AbilityBase _removeRule;

    public CombatEngine()
    {
        _sort = new SortAbility();
        _undo = new UndoAbility();
        _viewInbox = new ViewInboxAbility();
        _viewRules = new ViewRulesAbility();
        _addRule = new AddRuleAbility();
        _removeRule = new RemoveRuleAbility();
    }

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
            Console.WriteLine("5) Add Rule");
            Console.WriteLine("6) Remove Rule");
            Console.WriteLine("7) Quit");

            int choice = Input.ReadIntInRange("Choice: ", 1, 7);
            Console.WriteLine();

            if (choice == 1)
            {
                _sort.Use(state);
            }
            else if (choice == 2)
            {
                _undo.Use(state);
            }
            else if (choice == 3)
            {
                _viewInbox.Use(state);
            }
            else if (choice == 4)
            {
                _viewRules.Use(state);
            }
            else if (choice == 5)
            {
                _addRule.Use(state);
            }
            else if (choice == 6)
            {
                _removeRule.Use(state);
            }
            else
            {
                state.AddTurns(1);
                quit = true;
                break;
            }
        }

        bool won = state.InboxEmpty && state.TurnsUsed <= state.TurnLimit;
        ConsoleUi.PrintSummary(state, won, quit);
    }
}
