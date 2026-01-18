using System;

namespace JournalApp
{
    // Journal Program Main Menu.

    public class Program
    {
        private Journal _journal;

        public Program()
        {
            _journal = new Journal();
        }

        public static void Main(string[] args)
        {
            var app = new Program();
            app.Run();
        }

        private void Run()
        {
            bool exit = false;

            // Main Menu UI.
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("Journal Application");
                Console.WriteLine("1) Write a new entry");
                Console.WriteLine("2) Display the journal");
                Console.WriteLine("3) Save the journal");
                Console.WriteLine("4) Load the journal");
                Console.WriteLine("5) Delete an entry");
                Console.WriteLine("6) Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        WriteEntry();
                        break;
                    case "2":
                        DisplayJournal();
                        break;
                    case "3":
                        SaveJournal();
                        break;
                    case "4":
                        LoadJournal();
                        break;
                    case "5":
                        DeleteEntry();
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void WriteEntry()
        {
            string prompt = _journal.RandomPrompt();
            Console.WriteLine(prompt);
            Console.Write("Response: ");
            string response = Console.ReadLine();

            Console.Write("Mood (optional): ");
            string mood = Console.ReadLine();

            var entry = new Entry(DateTime.Now, prompt, response, mood);
            _journal.AddEntry(entry);

            Console.WriteLine("Entry added.");
        }

        private void DisplayJournal()
        {
            if (_journal.EntryCount == 0)
            {
                Console.WriteLine("No entries to display.");
                return;
            }

            for (int i = 0; i < _journal.EntryCount; i++)
            {
                Console.WriteLine($"--- Entry {i + 1} ---");
                Console.WriteLine(_journal.DisplayEntry(i));
            }
        }

        private void SaveJournal()
        {
            Console.Write("Enter filename: ");
            string fileName = Console.ReadLine();

            _journal.SaveToFile(fileName);
            Console.WriteLine("Journal saved.");
        }

        private void LoadJournal()
        {
            Console.Write("Enter filename: ");
            string fileName = Console.ReadLine();

            _journal.LoadFromFile(fileName);
            Console.WriteLine("Journal loaded.");
        }

        private void DeleteEntry()
        {
            Console.Write("Enter entry number to delete: ");
            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (_journal.DeleteEntry(index - 1))
                    Console.WriteLine("Entry deleted.");
                else
                    Console.WriteLine("Invalid entry number.");
            }
        }
    }
}
