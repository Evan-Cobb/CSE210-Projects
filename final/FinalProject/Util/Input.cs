namespace FinalProject.Util;

public static class Input
{
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (int.TryParse(input, out int value))
            {
                return value;
            }
            Console.WriteLine("Invalid number. Try again.");
        }
    }

    public static int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            int value = ReadInt(prompt);
            if (value >= min && value <= max)
            {
                return value;
            }
            Console.WriteLine($"Enter a number between {min} and {max}.");
        }
    }

    public static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            Console.WriteLine("Input cannot be empty.");
        }
    }
}
