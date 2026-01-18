using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JournalApp
{
    // Manages prompts and journal entries.
    public class Journal
    {
        private List<string> _prompts;
        private List<Entry> _entries;
        private Random _random;

        private readonly string _delimiter = "|~|";

        public Journal()
        {
            _random = new Random();
            _entries = new List<Entry>();

            _prompts = new List<string>
            {
                "Who was the most interesting person I interacted with today?",
                "What was the best part of my day?",
                "How did I see the hand of the Lord in my life today?",
                "What was the strongest emotion I felt today?",
                "If I had one thing I could do over today, what would it be?",
                "What did I learn today that surprised me?"
            };
        }

        // Added for safe iteration (see UML discussion)
        public int EntryCount => _entries.Count;

        public void AddEntry(Entry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            _entries.Add(entry);
        }

        public bool DeleteEntry(int index)
        {
            if (index < 0 || index >= _entries.Count)
                return false;

            _entries.RemoveAt(index);
            return true;
        }

        public string RandomPrompt()
        {
            if (_prompts.Count == 0)
                return string.Empty;

            int index = _random.Next(_prompts.Count);
            return _prompts[index];
        }

        public string DisplayEntry(int index)
        {
            if (index < 0 || index >= _entries.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _entries[index].Display();
        }

        public void SaveToFile(string filePath)
        {
            var lines = new List<string>();
            foreach (var entry in _entries)
            {
                lines.Add(entry.Serialize(_delimiter));
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            var loadedEntries = new List<Entry>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    loadedEntries.Add(Entry.Deserialize(line, _delimiter));
                }
            }

            _entries = loadedEntries;
        }
    }
}
