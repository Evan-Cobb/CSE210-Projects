using FinalProject.Core;
using FinalProject.Util;

namespace FinalProject.Abilities;

public class ViewRulesAbility : AbilityBase
{
    public ViewRulesAbility() : base("View Rules")
    {
    }

    public override void Use(GameState state)
    {
        state.AddTurns(1);
        ConsoleUi.PrintRules(state.RulePack);
    }
}
