using System;
using System.Text;

namespace JournalApp
{
    // A single journal entry.
    
    public class Entry
    {
        private DateTime _date;
        private string _prompt;
        private string _response;
        private string _mood;

        public Entry(DateTime date, string prompt, string response, string mood)
        {
            _date = date;
            _prompt = prompt ?? string.Empty;
            _response = response ?? string.Empty;
            _mood = mood ?? string.Empty;
        }

        // Returns a description of the entry.
        public string Display()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Date  : {_date:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Prompt: {_prompt}");
            sb.AppendLine($"Mood  : {_mood}");
            sb.AppendLine("Response:");
            sb.AppendLine(_response);
            return sb.ToString();
        }

        // Makes the entry easier for the program to display.
        internal string Serialize(string delimiter)
        {
            return $"{_date:o}{delimiter}{_prompt}{delimiter}{_response}{delimiter}{_mood}";
        }

        // Inverts Serialize() so that the program can manipulate the Entry objects properly.
        internal static Entry Deserialize(string line, string delimiter)
        {
            var parts = line.Split(new string[] { delimiter }, StringSplitOptions.None);
            if (parts.Length < 4)
            {
                throw new FormatException("Invalid entry format.");
            }

            DateTime date;
            if (!DateTime.TryParse(parts[0], null, System.Globalization.DateTimeStyles.RoundtripKind, out date))
            {
                date = DateTime.Now;
            }

            return new Entry(date, parts[1], parts[2], parts[3]);
        }
    }
}
