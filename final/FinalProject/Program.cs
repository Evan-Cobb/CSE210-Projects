using FinalProject.Core;
using FinalProject.Game;
using FinalProject.Rules;
using FinalProject.Util;

namespace FinalProject;

public static class Program
{
    private const int DemoSeed = 133742;

    public static int Main(string[] args)
    {
        if (args.Contains("--selftest", StringComparer.OrdinalIgnoreCase))
        {
            bool useDelay = !args.Contains("--selftest-nodelay", StringComparer.OrdinalIgnoreCase);
            return SelfTestRunner.Run(DemoSeed, useDelay);
        }

        ConsoleUi.PrintTitle();

        int seed = Input.ReadInt("Enter seed (integer): ");
        Difficulty difficulty = ReadDifficulty();

        Scenario scenario = ScenarioGenerator.Generate(seed, difficulty);

        Console.WriteLine();
        Console.WriteLine($"Generated {scenario.Items.Count} items.");
        Console.WriteLine($"Move limit: {scenario.MoveLimit}");

        RulePack rulePack = RuleWorkshop.BuildInitialPack(scenario.Vfs.FolderNames);
        GameState state = new GameState(scenario.Seed, scenario.Difficulty, scenario.MoveLimit, scenario.Vfs, rulePack, scenario.TruthTable);

        MoveChallengeEngine engine = new MoveChallengeEngine();
        engine.Run(state);

        return 0;
    }

    private static Difficulty ReadDifficulty()
    {
        Console.WriteLine();
        Console.WriteLine("Select difficulty:");
        Console.WriteLine("1) Easy");
        Console.WriteLine("2) Normal");
        Console.WriteLine("3) Hard");
        int choice = Input.ReadIntInRange("Choice: ", 1, 3);
        return choice switch
        {
            1 => Difficulty.Easy,
            2 => Difficulty.Normal,
            _ => Difficulty.Hard
        };
    }
}
