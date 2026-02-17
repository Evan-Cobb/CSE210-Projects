using FinalProject.World;

namespace FinalProject.Rules;

public class ExtensionRule : RuleBase
{
    public string Extension { get; }
    public string Destination { get; }

    public ExtensionRule(string extension, string destination)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException("Extension cannot be empty.", nameof(extension));
        }
        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("Destination cannot be empty.", nameof(destination));
        }

        Extension = NormalizeExtension(extension);
        Destination = destination;
    }

    public override bool IsMatch(VirtualFileItem item)
    {
        return string.Equals(item.Extension, Extension, StringComparison.OrdinalIgnoreCase);
    }

    public override string DestinationName(VirtualFileItem item)
    {
        return Destination;
    }

    public override string Describe()
    {
        return $"ExtensionRule: *{Extension} -> {Destination}";
    }

    private static string NormalizeExtension(string extension)
    {
        string value = extension.Trim();
        if (!value.StartsWith("."))
        {
            value = "." + value;
        }
        return value.ToLowerInvariant();
    }
}
