using FinalProject.Domain;

namespace FinalProject.Rules;

public sealed class ExtensionRule : RuleBase
{
    private readonly HashSet<string> _extensions;

    public ExtensionRule(string name, int priority, bool enabled, string destination, ActionKind actionKind, IEnumerable<string> extensions)
        : base(name, priority, enabled, destination, actionKind)
    {
        _extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (extensions == null)
        {
            return;
        }

        foreach (string ext in extensions)
        {
            string normalized = NormalizeExtension(ext);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                _extensions.Add(normalized);
            }
        }
    }

    protected override bool IsMatch(FileItem item, out string destinationSubPath)
    {
        destinationSubPath = Destination;

        if (_extensions.Count == 0)
        {
            return false;
        }

        return _extensions.Contains(NormalizeExtension(item.Extension));
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return string.Empty;
        }

        string trimmed = extension.Trim();
        return trimmed.StartsWith('.') ? trimmed.ToLowerInvariant() : "." + trimmed.ToLowerInvariant();
    }
}
