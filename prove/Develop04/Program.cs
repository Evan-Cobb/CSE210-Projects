using System;

public class Program
{
    public static void Main(string[] args)
    {
        var program = new Program();
        program.ShowMenu();
    }

    private void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Menu Options:");
            Console.WriteLine("  1. Start breathing activity");
            Console.WriteLine("  2. Start reflection activity");
            Console.WriteLine("  3. Start listing activity");
            Console.WriteLine("  4. Start custom activity");
            Console.WriteLine("  5. Quit");
            Console.Write("Select a choice from the menu: ");

            int choice = GetMenuChoice();

            if (choice == 5)
                return;

            Activity activity = choice switch
            {
                1 => new BreathingActivity(),
                2 => new ReflectionActivity(),
                3 => new ListingActivity(),
                4 => new CustomActivity(),
                _ => null
            };

            if (activity != null)
            {
                RunActivity(activity);
            }
        }
    }

    private int GetMenuChoice()
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 5)
            {
                return choice;
            }

            Console.Write("Invalid choice. Enter 1-5: ");
        }
    }

    private void RunActivity(Activity activity)
    {
        Console.Clear();
        activity.Start();

        Console.WriteLine();
        Console.Write("Press Enter to return to the menu...");
        Console.ReadLine();
    }
}
