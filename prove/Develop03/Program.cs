using System;
using System.Text.RegularExpressions;

namespace ScriptureMemorizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Scripture Memorizer");
            Console.WriteLine("===================");
            Console.WriteLine("Options:");
            Console.WriteLine("  1) Use an example scripture (John 3:16 or Proverbs 3:5-6)");
            Console.WriteLine("  2) Enter a custom scripture reference and paste text");
            Console.Write("Choose 1 or 2 (default 1): ");
            var choice = Console.ReadLine()?.Trim();

            Reference reference;
            string scriptureText;

            if (choice == "2")
            {
                reference = PromptForReferenceFromUser();
                scriptureText = PromptForScriptureText(reference);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Pick an example:");
                Console.WriteLine("  a) John 3:16");
                Console.WriteLine("  b) Proverbs 3:5-6");
                Console.Write("Choose a or b (default a): ");
                var pick = Console.ReadLine()?.Trim().ToLower();
                if (pick == "b")
                {
                    reference = new Reference("Proverbs", 3, 5, 6);
                    scriptureText = "Trust in the LORD with all thine heart; and lean not unto thine own understanding. " +
                                    "In all thy ways acknowledge him, and he shall direct thy paths.";
                }
                else
                {
                    reference = new Reference("John", 3, 16);
                    scriptureText = "For God so loved the world, that he gave his only begotten Son, " +
                                    "that whosoever believeth in him should not perish, but have everlasting life.";
                }
            }

            var scripture = new Scripture(reference, scriptureText);

            while (true)
            {
                Console.Clear();
                Console.WriteLine(scripture.GetDisplayText());
                Console.WriteLine();
                Console.WriteLine($"Visible words remaining: {scripture.VisibleWordCount()}");
                if (scripture.AllWordsHidden())
                {
                    Console.WriteLine("\nAll words hidden. Press Enter to exit.");
                    Console.ReadLine();
                    break;
                }

                Console.WriteLine("\nPress Enter to hide a few words, or type 'quit' then Enter to exit.");
                var cmd = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(cmd) && cmd.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                scripture.HideRandomWord();
            }

            Console.WriteLine("Goodbye!");
        }

        private static Reference PromptForReferenceFromUser()
        {
            while (true)
            {
                Console.WriteLine();
                Console.Write("Enter reference (examples: 'John 3:16' or 'Proverbs 3:5-6'): ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input blank — please enter a reference.");
                    continue;
                }

                var match = Regex.Match(input.Trim(), @"^([0-9A-Za-z\s\.]+)\s+(\d+)(?::(\d+)(?:-(\d+))?)?$");
                if (!match.Success)
                {
                    Console.WriteLine("Could not parse reference. Try format: 'John 3:16' or 'Proverbs 3:5-6'.");
                    continue;
                }

                var book = match.Groups[1].Value.Trim();
                var chapter = int.Parse(match.Groups[2].Value);

                if (match.Groups[3].Success)
                {
                    var startVerse = int.Parse(match.Groups[3].Value);
                    if (match.Groups[4].Success)
                    {
                        var endVerse = int.Parse(match.Groups[4].Value);
                        try
                        {
                            return new Reference(book, chapter, startVerse, endVerse);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Invalid range: {ex.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            return new Reference(book, chapter, startVerse);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Invalid verse: {ex.Message}");
                        }
                    }
                }
                else
                {
                    try
                    {
                        return new Reference(book, chapter, 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Invalid reference: {ex.Message}");
                    }
                }
            }
        }

        private static string PromptForScriptureText(Reference reference)
        {
            Console.WriteLine();
            Console.WriteLine($"Enter the text for {reference}. Paste entire text (multiple lines allowed).");
            Console.WriteLine("When finished, enter a single line containing only 'END' to finish input.");
            Console.WriteLine("(You may also enter a single-line text and press Enter, then 'END'.)");

            var lines = new System.Collections.Generic.List<string>();
            while (true)
            {
                var line = Console.ReadLine();
                if (line != null && line.Trim().Equals("END", StringComparison.OrdinalIgnoreCase))
                    break;
                if (line == null) break;
                lines.Add(line);
            }

            var text = string.Join(" ", lines).Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("No text provided — using placeholder '[text unavailable]'.");
                return "[text unavailable]";
            }
            return text;
        }
    }
}
