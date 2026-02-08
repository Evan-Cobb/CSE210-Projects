using FinalProject.Domain;

namespace FinalProject.Services;

public sealed class RulePackValidator
{
    private static readonly HashSet<string> AllowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "ExtensionRule",
        "NamePatternRule",
        "DateBucketRule",
        "FallbackRule"
    };

    public ValidationResult Validate(RulePack pack)
    {
        var result = new ValidationResult();

        if (pack == null)
        {
            result.Errors.Add("Rule pack is missing.");
            return result;
        }

        if (pack.Rules == null || pack.Rules.Count == 0)
        {
            result.Errors.Add("Rule pack must contain at least one rule.");
            return result;
        }

        for (int i = 0; i < pack.Rules.Count; i++)
        {
            RuleDefinition rule = pack.Rules[i];

            if (rule == null)
            {
                result.Errors.Add($"Rule at index {i} is missing.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(rule.Type) || !AllowedTypes.Contains(rule.Type))
            {
                result.Errors.Add($"Rule '{rule.Name}' has invalid type '{rule.Type}'.");
                continue;
            }

            if (rule.Action == ActionKind.EnsureDirectory)
            {
                result.Errors.Add($"Rule '{rule.Name}' cannot use EnsureDirectory as an action.");
            }

            if (Path.IsPathRooted(rule.Destination))
            {
                result.Errors.Add($"Rule '{rule.Name}' destination must be a relative path.");
            }

            if (!string.IsNullOrWhiteSpace(rule.Destination) && rule.Destination.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                result.Errors.Add($"Rule '{rule.Name}' destination contains invalid characters.");
            }

            if (rule.Type.Equals("ExtensionRule", StringComparison.OrdinalIgnoreCase))
            {
                if (rule.Extensions == null || rule.Extensions.Count == 0)
                {
                    result.Errors.Add($"ExtensionRule '{rule.Name}' must include at least one extension.");
                }
            }

            if (rule.Type.Equals("NamePatternRule", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(rule.Pattern))
                {
                    result.Errors.Add($"NamePatternRule '{rule.Name}' must include a pattern.");
                }
            }
        }

        return result;
    }

    public ValidationResult ValidateRootPath(string rootPath, IEnumerable<string> blockedDirectories, IFileSystem fileSystem)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(rootPath))
        {
            result.Errors.Add("Root path is required.");
            return result;
        }

        if (!fileSystem.DirectoryExists(rootPath))
        {
            result.Errors.Add($"Root path does not exist: {rootPath}");
            return result;
        }

        string fullRoot = fileSystem.GetFullPath(rootPath);
        StringComparison comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        foreach (string blocked in blockedDirectories ?? Array.Empty<string>())
        {
            if (string.IsNullOrWhiteSpace(blocked))
            {
                continue;
            }

            if (Path.IsPathRooted(blocked))
            {
                string blockedFull = fileSystem.GetFullPath(blocked);
                if (fullRoot.StartsWith(blockedFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), comparison))
                {
                    result.Errors.Add($"Root path cannot be inside blocked directory: {blocked}");
                }

                continue;
            }

            string[] segments = fullRoot.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (segments.Any(segment => segment.Equals(blocked, comparison)))
            {
                result.Errors.Add($"Root path cannot include blocked directory segment: {blocked}");
            }
        }

        return result;
    }
}
