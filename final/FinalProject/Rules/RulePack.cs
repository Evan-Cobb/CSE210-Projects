using FinalProject.World;

namespace FinalProject.Rules;

public class RulePack
{
    private readonly List<RuleBase> _rules;

    public RulePack()
    {
        _rules = new List<RuleBase>();
    }

    public IReadOnlyList<RuleBase> Rules => _rules.AsReadOnly();

    public void AddRule(RuleBase rule)
    {
        _rules.Add(rule);
    }

    public void RemoveAt(int index)
    {
        _rules.RemoveAt(index);
    }

    public void Move(int fromIndex, int toIndex)
    {
        RuleBase rule = _rules[fromIndex];
        _rules.RemoveAt(fromIndex);
        _rules.Insert(toIndex, rule);
    }

    public RuleBase Pick(VirtualFileItem item)
    {
        foreach (RuleBase rule in _rules)
        {
            if (rule.IsMatch(item))
            {
                return rule;
            }
        }
        return null;
    }

    public int CountFallbackRules()
    {
        int count = 0;
        foreach (RuleBase rule in _rules)
        {
            if (rule is FallbackRule)
            {
                count++;
            }
        }
        return count;
    }
}
