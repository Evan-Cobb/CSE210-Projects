using FinalProject.Core;
using FinalProject.Game;
using FinalProject.Util;

namespace FinalProject.Abilities;

public class AddRuleAbility : AbilityBase
{
    public AddRuleAbility() : base("Add Rule")
    {
    }

    public override void Use(GameState state)
    {
        state.AddTurns(1);

        List<string> destinations = new List<string>();
        foreach (string name in state.Vfs.FolderNames)
        {
            if (!string.Equals(name, "Inbox", StringComparison.OrdinalIgnoreCase))
            {
                destinations.Add(name);
            }
        }

        DraftPhase.AddRule(state.RulePack, destinations);
        ConsoleUi.PrintRules(state.RulePack);
    }
}
