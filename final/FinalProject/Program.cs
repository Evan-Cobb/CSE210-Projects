using FinalProject.Core;
using FinalProject.Game;
using FinalProject.Rules;
using FinalProject.Util;

namespace FinalProject;

public class Program
{
    public static void Main()
    {
        ConsoleUi.PrintTitle();

        int seed = Input.ReadInt("Enter seed (integer): ");
        Difficulty difficulty = ReadDifficulty();

        Scenario scenario = ScenarioGenerator.Generate(seed, difficulty);

        Console.WriteLine();
        Console.WriteLine($"Generated {scenario.Items.Count} items.");
        Console.WriteLine($"Turn limit: {scenario.TurnLimit}");

        RulePack rulePack = DraftPhase.BuildRulePack(scenario.Vfs.FolderNames);
        GameState state = new GameState(scenario.Seed, scenario.Difficulty, scenario.TurnLimit, scenario.Vfs, rulePack, scenario.TruthTable);

        CombatEngine engine = new CombatEngine();
        engine.Run(state);
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
