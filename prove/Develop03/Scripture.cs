using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScriptureMemorizer
{

    public class Scripture
    {
        private readonly Reference _reference;
        private readonly List<Word> _words;
        private readonly Random _random;

        public Scripture(Reference reference, string text)
        {
            _reference = reference ?? throw new ArgumentNullException(nameof(reference));
            _random = new Random();
            _words = Tokenize(text ?? string.Empty);
        }

        private static List<Word> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<Word>();

            var tokens = Regex.Split(text.Trim(), @"\s+")
                              .Where(t => !string.IsNullOrEmpty(t))
                              .ToList();

            return tokens.Select(t => new Word(t)).ToList();
        }

        public void HideRandomWord()
        {
            var visibleIndexes = _words
                .Select((w, i) => new { w, i })
                .Where(x => !x.w.IsHidden)
                .Select(x => x.i)
                .ToList();

            if (!visibleIndexes.Any()) return;

            int toHide = Math.Min(3, visibleIndexes.Count); 
            for (int k = 0; k < toHide; k++)
            {
                int pickIndex = _random.Next(visibleIndexes.Count);
                int wordIndex = visibleIndexes[pickIndex];
                _words[wordIndex].Hide();
                visibleIndexes.RemoveAt(pickIndex);
                if (!visibleIndexes.Any()) break;
            }
        }

        public bool AllWordsHidden()
        {
            return _words.All(w => w.IsHidden);
        }

        public string GetDisplayText()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_reference.ToString());
            sb.AppendLine();
            sb.Append(string.Join(" ", _words.Select(w => w.GetDisplayText())));
            return sb.ToString();
        }

        internal int VisibleWordCount() => _words.Count(w => !w.IsHidden);
    }
}
