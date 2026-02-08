using FinalProject.Domain;

namespace FinalProject.Services;

public sealed class PlanBuilder
{
    private readonly IFileSystem _fileSystem;
    private readonly StringComparison _pathComparison;
    private readonly StringComparer _pathComparer;

    public PlanBuilder(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _pathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        _pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
    }

    public PlanBuildResult Build(string rootPath, bool recursive, RulePack pack, List<IFileRule> rules)
    {
        var result = new PlanBuildResult();
        string rootFull = _fileSystem.GetFullPath(rootPath);

        HashSet<string> includeExtensions = NormalizeExtensions(pack.IncludeExtensions);
        HashSet<string> excludeExtensions = NormalizeExtensions(pack.ExcludeExtensions);

        List<string> destinationRoots = BuildDestinationRoots(rootFull, rules);
        BuildBlockedLists(pack.BlockedDirectories, out List<string> blockedNames, out List<string> blockedFullPaths);

        var stack = new Stack<string>();
        stack.Push(rootFull);

        while (stack.Count > 0)
        {
            string current = stack.Pop();

            if (IsBlockedDirectory(current, blockedNames, blockedFullPaths))
            {
                continue;
            }

            if (IsUnderAnyDestination(current, destinationRoots))
            {
                continue;
            }

            IEnumerable<string> files;
            try
            {
                files = _fileSystem.EnumerateFiles(current);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to read files in '{current}': {ex.Message}");
                continue;
            }

            foreach (string file in files)
            {
                FileItem item;
                try
                {
                    item = CreateFileItem(file);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Failed to read file '{file}': {ex.Message}");
                    continue;
                }

                string normalizedExtension = NormalizeExtension(item.Extension);
                if (includeExtensions.Count > 0 && !includeExtensions.Contains(normalizedExtension))
                {
                    continue;
                }

                if (excludeExtensions.Contains(normalizedExtension))
                {
                    continue;
                }

                RuleMatch match = SelectRule(item, rules);
                if (match == null)
                {
                    continue;
                }

                string destinationDirectory = string.IsNullOrWhiteSpace(match.DestinationSubPath)
                    ? rootFull
                    : Path.Combine(rootFull, match.DestinationSubPath);

                string destinationPath = Path.Combine(destinationDirectory, item.FileName);
                destinationPath = _fileSystem.GetFullPath(destinationPath);

                if (PathsEqual(destinationPath, item.FullPath))
                {
                    continue;
                }

                result.Items.Add(new PlanItem(item.FullPath, destinationPath, match.ActionKind, match.RuleName));
            }

            if (!recursive)
            {
                continue;
            }

            IEnumerable<string> directories;
            try
            {
                directories = _fileSystem.EnumerateDirectories(current);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to read directories in '{current}': {ex.Message}");
                continue;
            }

            foreach (string directory in directories)
            {
                if (IsBlockedDirectory(directory, blockedNames, blockedFullPaths))
                {
                    continue;
                }

                if (IsUnderAnyDestination(directory, destinationRoots))
                {
                    continue;
                }

                if (IsReparsePoint(directory))
                {
                    continue;
                }

                stack.Push(directory);
            }
        }

        return result;
    }

    private FileItem CreateFileItem(string path)
    {
        return new FileItem(
            _fileSystem.GetFullPath(path),
            _fileSystem.GetFileName(path),
            _fileSystem.GetFileNameWithoutExtension(path),
            _fileSystem.GetExtension(path),
            _fileSystem.GetCreationTimeUtc(path),
            _fileSystem.GetLastWriteTimeUtc(path));
    }

    private RuleMatch SelectRule(FileItem item, List<IFileRule> rules)
    {
        RuleMatch bestMatch = null;
        int bestPriority = int.MinValue;

        foreach (IFileRule rule in rules)
        {
            if (rule.TryMatch(item, out RuleMatch match))
            {
                if (rule.Priority > bestPriority)
                {
                    bestMatch = match;
                    bestPriority = rule.Priority;
                }
            }
        }

        return bestMatch;
    }

    private List<string> BuildDestinationRoots(string rootFull, List<IFileRule> rules)
    {
        var destinations = new List<string>();

        foreach (IFileRule rule in rules)
        {
            if (rule is RuleBase baseRule && !string.IsNullOrWhiteSpace(baseRule.Destination))
            {
                string full = _fileSystem.GetFullPath(Path.Combine(rootFull, baseRule.Destination));
                if (!destinations.Contains(full, _pathComparer))
                {
                    destinations.Add(full);
                }
            }
        }

        return destinations;
    }

    private bool IsBlockedDirectory(string path, List<string> blockedNames, List<string> blockedFullPaths)
    {
        string full = _fileSystem.GetFullPath(path);

        foreach (string blocked in blockedFullPaths)
        {
            if (IsUnderPath(full, blocked))
            {
                return true;
            }
        }

        string name = _fileSystem.GetFileName(full);
        foreach (string blockedName in blockedNames)
        {
            if (name.Equals(blockedName, _pathComparison))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsUnderAnyDestination(string path, List<string> destinationRoots)
    {
        string full = _fileSystem.GetFullPath(path);
        foreach (string destination in destinationRoots)
        {
            if (IsUnderPath(full, destination))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsUnderPath(string candidate, string root)
    {
        string normalizedRoot = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (candidate.Equals(normalizedRoot, _pathComparison))
        {
            return true;
        }

        string separatorRoot = normalizedRoot + Path.DirectorySeparatorChar;
        if (candidate.StartsWith(separatorRoot, _pathComparison))
        {
            return true;
        }

        string altSeparatorRoot = normalizedRoot + Path.AltDirectorySeparatorChar;
        return candidate.StartsWith(altSeparatorRoot, _pathComparison);
    }

    private bool IsReparsePoint(string path)
    {
        try
        {
            return (_fileSystem.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;
        }
        catch
        {
            return false;
        }
    }

    private HashSet<string> NormalizeExtensions(IEnumerable<string> extensions)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (extensions == null)
        {
            return set;
        }

        foreach (string extension in extensions)
        {
            string normalized = NormalizeExtension(extension);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                set.Add(normalized);
            }
        }

        return set;
    }

    private void BuildBlockedLists(IEnumerable<string> blockedDirectories, out List<string> blockedNames, out List<string> blockedFullPaths)
    {
        blockedNames = new List<string>();
        blockedFullPaths = new List<string>();

        if (blockedDirectories == null)
        {
            return;
        }

        foreach (string blocked in blockedDirectories)
        {
            if (string.IsNullOrWhiteSpace(blocked))
            {
                continue;
            }

            if (Path.IsPathRooted(blocked))
            {
                blockedFullPaths.Add(_fileSystem.GetFullPath(blocked));
            }
            else
            {
                blockedNames.Add(blocked.Trim());
            }
        }
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

    private bool PathsEqual(string left, string right) => string.Equals(left, right, _pathComparison);
}
