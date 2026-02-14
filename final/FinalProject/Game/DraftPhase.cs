using FinalProject.Rules;
using FinalProject.Util;

namespace FinalProject.Game;

public static class DraftPhase
{
    public static RulePack BuildRulePack(IReadOnlyList<string> folderNames)
    {
        List<string> destinations = new List<string>();
        foreach (string name in folderNames)
        {
            if (!string.Equals(name, "Inbox", StringComparison.OrdinalIgnoreCase))
            {
                destinations.Add(name);
            }
        }

        RulePack pack = new RulePack();

        while (true)
        {
            Console.WriteLine();
            ConsoleUi.PrintRules(pack);
            Console.WriteLine();
            Console.WriteLine("Draft Phase:");
            Console.WriteLine("1) Add rule");
            Console.WriteLine("2) Remove rule");
            Console.WriteLine("3) Reorder rules");
            Console.WriteLine("4) Done");

            int choice = Input.ReadIntInRange("Choice: ", 1, 4);

            if (choice == 1)
            {
                AddRule(pack, destinations);
            }
            else if (choice == 2)
            {
                RemoveRule(pack);
            }
            else if (choice == 3)
            {
                ReorderRules(pack);
            }
            else
            {
                if (pack.CountFallbackRules() != 1)
                {
                    Console.WriteLine("You must have exactly 1 fallback rule.");
                    continue;
                }
                return pack;
            }
        }
    }

    internal static void AddRule(RulePack pack, IReadOnlyList<string> destinations)
    {
        Console.WriteLine();
        Console.WriteLine("Select rule type:");
        Console.WriteLine("1) ExtensionRule");
        Console.WriteLine("2) NamePatternRule");
        Console.WriteLine("3) DateBucketRule");
        Console.WriteLine("4) FallbackRule");

        int type = Input.ReadIntInRange("Choice: ", 1, 4);
        if (type == 4 && pack.CountFallbackRules() >= 1)
        {
            Console.WriteLine("A fallback rule already exists.");
            return;
        }

        string destination = PickDestination(destinations);

        RuleBase newRule;
        if (type == 1)
        {
            string extension = Input.ReadNonEmptyString("Extension (e.g. .png): ");
            newRule = new ExtensionRule(extension, destination);
        }
        else if (type == 2)
        {
            Console.WriteLine("Match type:");
            Console.WriteLine("1) Contains");
            Console.WriteLine("2) StartsWith");
            Console.WriteLine("3) EndsWith");
            int matchChoice = Input.ReadIntInRange("Choice: ", 1, 3);
            NameMatchType matchType = (NameMatchType)matchChoice;
            string pattern = Input.ReadNonEmptyString("Pattern: ");
            newRule = new NamePatternRule(pattern, matchType, destination);
        }
        else if (type == 3)
        {
            Console.WriteLine("Date field:");
            Console.WriteLine("1) CreatedUtc");
            Console.WriteLine("2) ModifiedUtc");
            int fieldChoice = Input.ReadIntInRange("Choice: ", 1, 2);
            DateField field = (DateField)fieldChoice;

            Console.WriteLine("Bucket type:");
            Console.WriteLine("1) Year");
            Console.WriteLine("2) YearMonth");
            int bucketChoice = Input.ReadIntInRange("Choice: ", 1, 2);
            DateBucketType bucketType = (DateBucketType)bucketChoice;

            int year = Input.ReadIntInRange("Year (e.g. 2024): ", 1990, 2100);
            int month = 1;
            if (bucketType == DateBucketType.YearMonth)
            {
                month = Input.ReadIntInRange("Month (1-12): ", 1, 12);
            }

            newRule = new DateBucketRule(field, bucketType, year, month, destination);
        }
        else
        {
            newRule = new FallbackRule(destination);
        }

        int insertAt = pack.Rules.Count;
        if (pack.Rules.Count > 0)
        {
            Console.WriteLine($"Select priority (1 = highest, {pack.Rules.Count + 1} = lowest):");
            insertAt = Input.ReadIntInRange("Priority: ", 1, pack.Rules.Count + 1) - 1;
        }

        pack.AddRule(newRule);
        if (insertAt != pack.Rules.Count - 1)
        {
            pack.Move(pack.Rules.Count - 1, insertAt);
        }
    }

    private static void RemoveRule(RulePack pack)
    {
        if (pack.Rules.Count == 0)
        {
            Console.WriteLine("No rules to remove.");
            return;
        }

        ConsoleUi.PrintRules(pack);
        int index = Input.ReadIntInRange("Remove which rule: ", 1, pack.Rules.Count);
        pack.RemoveAt(index - 1);
    }

    private static void ReorderRules(RulePack pack)
    {
        if (pack.Rules.Count < 2)
        {
            Console.WriteLine("Need at least 2 rules to reorder.");
            return;
        }

        ConsoleUi.PrintRules(pack);
        int from = Input.ReadIntInRange("Move rule #: ", 1, pack.Rules.Count);
        int to = Input.ReadIntInRange("New position #: ", 1, pack.Rules.Count);
        pack.Move(from - 1, to - 1);
    }

    private static string PickDestination(IReadOnlyList<string> destinations)
    {
        Console.WriteLine("Destination folder:");
        for (int i = 0; i < destinations.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {destinations[i]}");
        }
        int choice = Input.ReadIntInRange("Choice: ", 1, destinations.Count);
        return destinations[choice - 1];
    }
}
