using FinalProject.Core;
using FinalProject.Rules;
using FinalProject.Util;

namespace FinalProject.Abilities;

public class RemoveRuleAbility : AbilityBase
{
    public RemoveRuleAbility() : base("Remove Rule")
    {
    }

    public override void Use(GameState state)
    {
        state.AddTurns(1);

        if (state.RulePack.Rules.Count == 0)
        {
            Console.WriteLine("No rules to remove.");
            return;
        }

        ConsoleUi.PrintRules(state.RulePack);
        int index = Input.ReadIntInRange("Remove which rule: ", 1, state.RulePack.Rules.Count);
        RuleBase rule = state.RulePack.Rules[index - 1];
        if (rule is FallbackRule && state.RulePack.CountFallbackRules() == 1)
        {
            Console.WriteLine("Cannot remove the only fallback rule during combat.");
            return;
        }

        state.RulePack.RemoveAt(index - 1);
        Console.WriteLine("Rule removed.");
    }
}
