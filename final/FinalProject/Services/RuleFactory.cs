using FinalProject.Domain;
using FinalProject.Rules;

namespace FinalProject.Services;

public sealed class RuleFactory
{
    public List<IFileRule> CreateRules(RulePack pack)
    {
        var rules = new List<IFileRule>();

        foreach (RuleDefinition definition in pack.Rules)
        {
            if (definition == null)
            {
                continue;
            }

            IFileRule rule = CreateRule(definition);
            if (rule != null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    private static IFileRule CreateRule(RuleDefinition definition)
    {
        string type = definition.Type?.Trim() ?? string.Empty;

        if (type.Equals("ExtensionRule", StringComparison.OrdinalIgnoreCase))
        {
            return new ExtensionRule(
                definition.Name,
                definition.Priority,
                definition.Enabled,
                definition.Destination,
                definition.Action,
                definition.Extensions);
        }

        if (type.Equals("NamePatternRule", StringComparison.OrdinalIgnoreCase))
        {
            return new NamePatternRule(
                definition.Name,
                definition.Priority,
                definition.Enabled,
                definition.Destination,
                definition.Action,
                definition.Pattern,
                definition.Match);
        }

        if (type.Equals("DateBucketRule", StringComparison.OrdinalIgnoreCase))
        {
            return new DateBucketRule(
                definition.Name,
                definition.Priority,
                definition.Enabled,
                definition.Destination,
                definition.Action,
                definition.Bucket,
                definition.DateSource);
        }

        if (type.Equals("FallbackRule", StringComparison.OrdinalIgnoreCase))
        {
            return new FallbackRule(
                definition.Name,
                definition.Priority,
                definition.Enabled,
                definition.Destination,
                definition.Action);
        }

        return null;
    }
}
