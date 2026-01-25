using System;

namespace ScriptureMemorizer
{

    public class Reference
    {
        private readonly string _book;
        private readonly int _chapter;
        private readonly int _startVerse;
        private readonly int _endVerse; // endVerse = startVerse for single verse

        public Reference(string book, int chapter, int verse)
        {
            if (string.IsNullOrWhiteSpace(book)) throw new ArgumentException("book required", nameof(book));
            if (chapter <= 0) throw new ArgumentOutOfRangeException(nameof(chapter));
            if (verse <= 0) throw new ArgumentOutOfRangeException(nameof(verse));

            _book = book.Trim();
            _chapter = chapter;
            _startVerse = verse;
            _endVerse = verse;
        }

        public Reference(string book, int chapter, int startVerse, int endVerse)
        {
            if (string.IsNullOrWhiteSpace(book)) throw new ArgumentException("book required", nameof(book));
            if (chapter <= 0) throw new ArgumentOutOfRangeException(nameof(chapter));
            if (startVerse <= 0 || endVerse <= 0) throw new ArgumentOutOfRangeException("verses must be positive");
            if (endVerse < startVerse) throw new ArgumentException("endVerse must be >= startVerse");

            _book = book.Trim();
            _chapter = chapter;
            _startVerse = startVerse;
            _endVerse = endVerse;
        }

        public string Book => _book;
        public int Chapter => _chapter;
        public int StartVerse => _startVerse;
        public int EndVerse => _endVerse;

        public bool IsRange() => _endVerse > _startVerse;
        public bool IsSingleVerse() => _endVerse == _startVerse;

        public override string ToString()
        {
            if (IsSingleVerse()) return $"{_book} {_chapter}:{_startVerse}";
            return $"{_book} {_chapter}:{_startVerse}-{_endVerse}";
        }
    }
}
