using System.Text;

namespace ScriptureMemorizer
{

    public class Word
    {
        private readonly string _text;
        private bool _isHidden;

        public Word(string text)
        {
            _text = text ?? string.Empty;
            _isHidden = false;
        }

        public string Text => _text;
        public bool IsHidden => _isHidden;

        public void Hide()
        {
            _isHidden = true;
        }

        public string GetDisplayText()
        {
            if (!_isHidden) return _text;

            var sb = new StringBuilder(_text.Length);
            foreach (char c in _text)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append('_');
                else
                    sb.Append(c); // preserve punctuation
            }
            return sb.ToString();
        }
    }
}
