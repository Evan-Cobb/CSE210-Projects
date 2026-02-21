using FinalProject.Core;
using FinalProject.Rules;
using FinalProject.World;
using System.Diagnostics;
using System.Threading;

namespace FinalProject.Game;

public static class SelfTestRunner
{
    private const int DelayMs = 180;

    public static int Run(int demoSeed, bool useDelay = true)
    {
        List<SelfTestCase> tests = new List<SelfTestCase>
        {
            new SelfTestCase("Deterministic scenario generation", DeterministicScenarioGeneration),
            new SelfTestCase("Difficulty ranges and move-limit formula", DifficultyRangesAndMoveLimitFormula),
            new SelfTestCase("Seed determinism matrix", SeedDeterminismMatrix),
            new SelfTestCase("Rule priority uses first match", RulePriorityUsesFirstMatch),
            new SelfTestCase("ExtensionRule matches case-insensitively", ExtensionRuleCaseInsensitive),
            new SelfTestCase("NamePatternRule supports Contains/StartsWith/EndsWith", NamePatternRuleModes),
            new SelfTestCase("DateBucketRule supports Year and YearMonth", DateBucketRuleModes),
            new SelfTestCase("Correct sort bookkeeping", CorrectSortBookkeeping),
            new SelfTestCase("Incorrect sort penalty bookkeeping", IncorrectSortPenaltyBookkeeping),
            new SelfTestCase("Undo restores state after correct sort", UndoAfterCorrectSortRestoresState),
            new SelfTestCase("Undo restores state after incorrect sort", UndoAfterIncorrectSortRestoresState),
            new SelfTestCase("Undo on empty stack costs one move", UndoEmptyStackCostsMove),
            new SelfTestCase("Perfect rule pack clears inbox within move limit", PerfectRulePackClearsInbox),
            new SelfTestCase("Undo restores one item after full clear", UndoRestoresItemAfterFullClear)
        };

        int passed = 0;
        int failed = 0;

        Console.WriteLine("Self-check suite");
        Console.WriteLine($"Seed: {demoSeed}, Difficulty: Easy");
        Console.WriteLine();

        foreach (SelfTestCase test in tests)
        {
            Console.WriteLine($"[RUN ] {test.Name}");
            Pause(useDelay);

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                TestResult result = test.Run(demoSeed);
                stopwatch.Stop();

                if (result.Passed)
                {
                    passed++;
                    Console.WriteLine($"[PASS] {test.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                }
                else
                {
                    failed++;
                    Console.WriteLine($"[FAIL] {test.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                }

                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    Console.WriteLine($"       {result.Message}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                failed++;
                Console.WriteLine($"[FAIL] {test.Name} ({stopwatch.ElapsedMilliseconds} ms)");
                Console.WriteLine($"       Exception: {ex.Message}");
            }

            Pause(useDelay);
            Console.WriteLine();
        }

        Console.WriteLine($"Summary: {passed} passed, {failed} failed");
        if (failed == 0)
        {
            Console.WriteLine("PASS");
            return 0;
        }

        Console.WriteLine("FAIL");
        return 1;
    }

    private static TestResult DeterministicScenarioGeneration(int seed)
    {
        Scenario first = ScenarioGenerator.Generate(seed, Difficulty.Easy);
        Scenario second = ScenarioGenerator.Generate(seed, Difficulty.Easy);

        return CompareScenario(first, second, "Easy");
    }

    private static TestResult DifficultyRangesAndMoveLimitFormula(int seed)
    {
        List<string> failures = new List<string>();
        CheckDifficulty(seed, Difficulty.Easy, 8, 12, 6, failures);
        CheckDifficulty(seed, Difficulty.Normal, 12, 18, 4, failures);
        CheckDifficulty(seed, Difficulty.Hard, 18, 25, 2, failures);

        if (failures.Count > 0)
        {
            return new TestResult(false, string.Join(" | ", failures));
        }

        return new TestResult(true, "All difficulty ranges and move limits validated.");
    }

    private static void CheckDifficulty(int seed, Difficulty difficulty, int minItems, int maxItems, int moveOffset, List<string> failures)
    {
        Scenario scenario = ScenarioGenerator.Generate(seed, difficulty);
        int count = scenario.Items.Count;
        if (count < minItems || count > maxItems)
        {
            failures.Add($"{difficulty} item count {count} out of range {minItems}-{maxItems}.");
        }

        int expectedMoveLimit = count + moveOffset;
        if (scenario.MoveLimit != expectedMoveLimit)
        {
            failures.Add($"{difficulty} move limit {scenario.MoveLimit} != {expectedMoveLimit}.");
        }
    }

    private static TestResult SeedDeterminismMatrix(int demoSeed)
    {
        int[] seeds = { demoSeed, 42, -17, 2026 };
        Difficulty[] difficulties = { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard };

        foreach (int seed in seeds)
        {
            foreach (Difficulty difficulty in difficulties)
            {
                Scenario first = ScenarioGenerator.Generate(seed, difficulty);
                Scenario second = ScenarioGenerator.Generate(seed, difficulty);
                TestResult comparison = CompareScenario(first, second, difficulty.ToString());
                if (!comparison.Passed)
                {
                    return new TestResult(false, $"Seed {seed}, {difficulty}: {comparison.Message}");
                }
            }
        }

        return new TestResult(true, "Compared 12 scenario pairs across seeds and difficulties.");
    }

    private static TestResult CompareScenario(Scenario first, Scenario second, string label)
    {
        if (first.Items.Count != second.Items.Count)
        {
            return new TestResult(false, $"{label}: item counts differ.");
        }

        if (first.MoveLimit != second.MoveLimit)
        {
            return new TestResult(false, $"{label}: move limits differ.");
        }

        for (int i = 0; i < first.Items.Count; i++)
        {
            VirtualFileItem a = first.Items[i];
            VirtualFileItem b = second.Items[i];
            if (!string.Equals(a.Name, b.Name, StringComparison.Ordinal))
            {
                return new TestResult(false, $"{label}: name mismatch at index {i + 1}.");
            }

            if (a.CreatedUtc != b.CreatedUtc || a.ModifiedUtc != b.ModifiedUtc)
            {
                return new TestResult(false, $"{label}: date mismatch at index {i + 1}.");
            }
        }

        return new TestResult(true, $"{label}: deterministic.");
    }

    private static TestResult RulePriorityUsesFirstMatch(int seed)
    {
        RulePack pack = new RulePack();
        pack.AddRule(new FallbackRule("Misc"));
        pack.AddRule(new ExtensionRule(".pdf", "Documents"));

        GameState state = CreateSingleItemState("resume.pdf", "Documents", pack);
        VirtualFileItem item = state.Vfs.GetFolderItems("Inbox")[0];
        SortResult result = state.ApplySort(item);

        bool success = !result.IsCorrect &&
            string.Equals(result.Destination, "Misc", StringComparison.OrdinalIgnoreCase) &&
            result.RuleDescription.StartsWith("FallbackRule", StringComparison.Ordinal);

        return new TestResult(success, $"Destination: {result.Destination}, rule: {result.RuleDescription}.");
    }

    private static TestResult ExtensionRuleCaseInsensitive(int seed)
    {
        ExtensionRule rule = new ExtensionRule(".pdf", "Documents");
        VirtualFileItem item = new VirtualFileItem("Resume.PDF", DateTime.UtcNow, DateTime.UtcNow);

        bool success = rule.IsMatch(item);
        return new TestResult(success, "Rule matched Resume.PDF with .pdf.");
    }

    private static TestResult NamePatternRuleModes(int seed)
    {
        VirtualFileItem item = new VirtualFileItem("IMG_1234.png", DateTime.UtcNow, DateTime.UtcNow);

        NamePatternRule contains = new NamePatternRule("123", NameMatchType.Contains, "Pictures");
        NamePatternRule startsWith = new NamePatternRule("img_", NameMatchType.StartsWith, "Pictures");
        NamePatternRule endsWith = new NamePatternRule(".PNG", NameMatchType.EndsWith, "Pictures");
        NamePatternRule negative = new NamePatternRule("zzz", NameMatchType.StartsWith, "Pictures");

        bool success = contains.IsMatch(item) && startsWith.IsMatch(item) && endsWith.IsMatch(item) && !negative.IsMatch(item);
        return new TestResult(success, "Contains/StartsWith/EndsWith pass; negative case fails as expected.");
    }

    private static TestResult DateBucketRuleModes(int seed)
    {
        DateTime created = new DateTime(2024, 5, 10, 12, 0, 0, DateTimeKind.Utc);
        DateTime modified = new DateTime(2025, 1, 20, 12, 0, 0, DateTimeKind.Utc);
        VirtualFileItem item = new VirtualFileItem("notes.txt", created, modified);

        DateBucketRule createdYear = new DateBucketRule(DateField.CreatedUtc, DateBucketType.Year, 2024, 1, "Documents");
        DateBucketRule createdYearMonth = new DateBucketRule(DateField.CreatedUtc, DateBucketType.YearMonth, 2024, 5, "Documents");
        DateBucketRule modifiedYearMonth = new DateBucketRule(DateField.ModifiedUtc, DateBucketType.YearMonth, 2025, 1, "Documents");
        DateBucketRule negative = new DateBucketRule(DateField.ModifiedUtc, DateBucketType.Year, 2024, 1, "Documents");

        bool success = createdYear.IsMatch(item) && createdYearMonth.IsMatch(item) && modifiedYearMonth.IsMatch(item) && !negative.IsMatch(item);
        return new TestResult(success, "Created/Modified with Year/YearMonth behaves correctly.");
    }

    private static TestResult CorrectSortBookkeeping(int seed)
    {
        RulePack pack = new RulePack();
        pack.AddRule(new ExtensionRule(".png", "Pictures"));
        pack.AddRule(new FallbackRule("Misc"));

        GameState state = CreateSingleItemState("IMG_1111.png", "Pictures", pack);
        VirtualFileItem item = state.Vfs.GetFolderItems("Inbox")[0];
        SortResult result = state.ApplySort(item);

        bool success = result.IsCorrect &&
            result.MoveCost == 1 &&
            state.Score == 10 &&
            state.MovesUsed == 1 &&
            state.WrongSorts == 0 &&
            state.InboxCount == 0 &&
            state.UndoStack.Count == 1;

        return new TestResult(success, $"Score {state.Score}, moves {state.MovesUsed}, wrong {state.WrongSorts}.");
    }

    private static TestResult IncorrectSortPenaltyBookkeeping(int seed)
    {
        RulePack pack = BuildAlwaysWrongRulePack();
        GameState state = CreateSingleItemState("resume.pdf", "Documents", pack);

        VirtualFileItem item = state.Vfs.GetFolderItems("Inbox")[0];
        SortResult result = state.ApplySort(item);

        bool success = !result.IsCorrect &&
            result.MoveCost == 2 &&
            state.Score == 0 &&
            state.MovesUsed == 2 &&
            state.WrongSorts == 1 &&
            state.InboxCount == 0 &&
            state.UndoStack.Count == 1;

        return new TestResult(success, $"Move cost {result.MoveCost}, score {state.Score}, wrong {state.WrongSorts}.");
    }

    private static TestResult UndoAfterCorrectSortRestoresState(int seed)
    {
        RulePack pack = new RulePack();
        pack.AddRule(new ExtensionRule(".png", "Pictures"));
        pack.AddRule(new FallbackRule("Misc"));

        GameState state = CreateSingleItemState("IMG_1111.png", "Pictures", pack);
        VirtualFileItem item = state.Vfs.GetFolderItems("Inbox")[0];
        state.ApplySort(item);

        bool undone = state.ApplyUndo();
        bool success = undone &&
            state.InboxCount == 1 &&
            state.Score == 0 &&
            state.WrongSorts == 0 &&
            state.MovesUsed == 2;

        return new TestResult(success, $"Undo {undone}, inbox {state.InboxCount}, moves {state.MovesUsed}.");
    }

    private static TestResult UndoAfterIncorrectSortRestoresState(int seed)
    {
        RulePack pack = BuildAlwaysWrongRulePack();
        GameState state = CreateSingleItemState("resume.pdf", "Documents", pack);

        VirtualFileItem item = state.Vfs.GetFolderItems("Inbox")[0];
        state.ApplySort(item);
        bool undone = state.ApplyUndo();

        bool success = undone &&
            state.InboxCount == 1 &&
            state.Score == 0 &&
            state.WrongSorts == 0 &&
            state.MovesUsed == 3;

        return new TestResult(success, $"Undo {undone}, wrong sorts {state.WrongSorts}, moves {state.MovesUsed}.");
    }

    private static TestResult UndoEmptyStackCostsMove(int seed)
    {
        RulePack pack = new RulePack();
        pack.AddRule(new FallbackRule("Misc"));
        GameState state = CreateSingleItemState("notes.txt", "Documents", pack);

        bool undone = state.ApplyUndo();
        bool success = !undone && state.MovesUsed == 1;
        return new TestResult(success, $"Undo {undone}, moves {state.MovesUsed}.");
    }

    private static TestResult PerfectRulePackClearsInbox(int seed)
    {
        GameState state = CreateState(seed, BuildPerfectRulePack(), Difficulty.Easy);

        while (!state.InboxEmpty && state.MovesUsed <= state.MoveLimit)
        {
            IReadOnlyList<VirtualFileItem> inbox = state.Vfs.GetFolderItems("Inbox");
            state.ApplySort(inbox[0]);
        }

        bool success = state.InboxEmpty && state.MovesUsed <= state.MoveLimit;
        return new TestResult(success, $"Moves used: {state.MovesUsed}/{state.MoveLimit}.");
    }

    private static TestResult UndoRestoresItemAfterFullClear(int seed)
    {
        GameState state = CreateState(seed, BuildPerfectRulePack(), Difficulty.Easy);

        while (!state.InboxEmpty && state.MovesUsed <= state.MoveLimit)
        {
            IReadOnlyList<VirtualFileItem> inbox = state.Vfs.GetFolderItems("Inbox");
            state.ApplySort(inbox[0]);
        }

        if (!state.InboxEmpty || state.MovesUsed > state.MoveLimit)
        {
            return new TestResult(false, "Could not clear inbox before testing undo.");
        }

        bool undone = state.ApplyUndo();
        bool success = undone && state.InboxCount == 1;
        return new TestResult(success, $"Undo returned {undone}, inbox count {state.InboxCount}.");
    }

    private static GameState CreateState(int seed, RulePack pack, Difficulty difficulty)
    {
        Scenario scenario = ScenarioGenerator.Generate(seed, difficulty);
        return new GameState(
            scenario.Seed,
            scenario.Difficulty,
            scenario.MoveLimit,
            scenario.Vfs,
            pack,
            scenario.TruthTable);
    }

    private static GameState CreateSingleItemState(string fileName, string correctFolder, RulePack pack)
    {
        VirtualFileSystem vfs = new VirtualFileSystem(ScenarioGenerator.DefaultFolderNames);
        VirtualFileItem item = new VirtualFileItem(fileName, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        vfs.AddToFolder("Inbox", item);

        Dictionary<Guid, string> truth = new Dictionary<Guid, string>
        {
            [item.Id] = correctFolder
        };

        return new GameState(0, Difficulty.Easy, 10, vfs, pack, truth);
    }

    private static void Pause(bool useDelay)
    {
        if (useDelay)
        {
            Thread.Sleep(DelayMs);
        }
    }

    private static RulePack BuildPerfectRulePack()
    {
        RulePack pack = new RulePack();
        pack.AddRule(new ExtensionRule(".png", "Pictures"));
        pack.AddRule(new ExtensionRule(".jpg", "Pictures"));
        pack.AddRule(new ExtensionRule(".jpeg", "Pictures"));
        pack.AddRule(new ExtensionRule(".gif", "Pictures"));
        pack.AddRule(new ExtensionRule(".svg", "Pictures"));

        pack.AddRule(new ExtensionRule(".pdf", "Documents"));
        pack.AddRule(new ExtensionRule(".txt", "Documents"));
        pack.AddRule(new ExtensionRule(".docx", "Documents"));
        pack.AddRule(new ExtensionRule(".xlsx", "Documents"));
        pack.AddRule(new ExtensionRule(".pptx", "Documents"));
        pack.AddRule(new ExtensionRule(".md", "Documents"));
        pack.AddRule(new ExtensionRule(".csv", "Documents"));

        pack.AddRule(new ExtensionRule(".zip", "Archives"));
        pack.AddRule(new ExtensionRule(".7z", "Archives"));
        pack.AddRule(new ExtensionRule(".rar", "Archives"));
        pack.AddRule(new ExtensionRule(".tar", "Archives"));
        pack.AddRule(new ExtensionRule(".gz", "Archives"));
        pack.AddRule(new ExtensionRule(".tgz", "Archives"));

        pack.AddRule(new ExtensionRule(".deb", "Installers"));
        pack.AddRule(new ExtensionRule(".msi", "Installers"));
        pack.AddRule(new ExtensionRule(".exe", "Installers"));
        pack.AddRule(new ExtensionRule(".pkg", "Installers"));

        pack.AddRule(new ExtensionRule(".mp3", "Audio"));
        pack.AddRule(new ExtensionRule(".wav", "Audio"));
        pack.AddRule(new ExtensionRule(".flac", "Audio"));
        pack.AddRule(new ExtensionRule(".m4a", "Audio"));
        pack.AddRule(new ExtensionRule(".ogg", "Audio"));

        pack.AddRule(new ExtensionRule(".mp4", "Video"));
        pack.AddRule(new ExtensionRule(".mov", "Video"));
        pack.AddRule(new ExtensionRule(".avi", "Video"));
        pack.AddRule(new ExtensionRule(".mkv", "Video"));
        pack.AddRule(new ExtensionRule(".webm", "Video"));

        pack.AddRule(new ExtensionRule(".cs", "Code"));
        pack.AddRule(new ExtensionRule(".py", "Code"));
        pack.AddRule(new ExtensionRule(".js", "Code"));
        pack.AddRule(new ExtensionRule(".ts", "Code"));
        pack.AddRule(new ExtensionRule(".java", "Code"));
        pack.AddRule(new ExtensionRule(".html", "Code"));
        pack.AddRule(new ExtensionRule(".css", "Code"));
        pack.AddRule(new ExtensionRule(".json", "Code"));
        pack.AddRule(new ExtensionRule(".sql", "Code"));
        pack.AddRule(new ExtensionRule(".ps1", "Code"));

        pack.AddRule(new FallbackRule("Misc"));
        return pack;
    }

    private static RulePack BuildAlwaysWrongRulePack()
    {
        RulePack pack = new RulePack();
        pack.AddRule(new FallbackRule("Misc"));
        return pack;
    }

    private readonly record struct SelfTestCase(string Name, Func<int, TestResult> Run);
    private readonly record struct TestResult(bool Passed, string Message);
}

