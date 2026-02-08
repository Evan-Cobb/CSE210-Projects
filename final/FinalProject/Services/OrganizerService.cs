using FinalProject.Domain;

namespace FinalProject.Services;

public sealed class OrganizerService
{
    private readonly IFileSystem _fileSystem;
    private readonly PlanBuilder _planBuilder;
    private readonly List<IFileAction> _actions;
    private readonly ConflictResolver _conflictResolver;

    public OrganizerService(IFileSystem fileSystem, PlanBuilder planBuilder, IEnumerable<IFileAction> actions)
    {
        _fileSystem = fileSystem;
        _planBuilder = planBuilder;
        _actions = actions?.ToList() ?? new List<IFileAction>();
        _conflictResolver = new ConflictResolver(fileSystem);
    }

    public PlanBuildResult BuildPlan(string rootPath, bool recursive, RulePack pack, List<IFileRule> rules)
    {
        return _planBuilder.Build(rootPath, recursive, pack, rules);
    }

    public OrganizerResult ExecutePlan(IEnumerable<PlanItem> planItems, ConflictPolicy conflictPolicy, string rootPath)
    {
        int succeeded = 0;
        int failed = 0;
        int skipped = 0;
        var warnings = new List<string>();

        var runLog = new RunLog
        {
            RunId = Guid.NewGuid(),
            TimestampUtc = DateTime.UtcNow,
            RootPath = rootPath,
            ConflictPolicy = conflictPolicy,
            Entries = new List<RunLogEntry>()
        };

        IFileAction ensureDirectory = GetAction(ActionKind.EnsureDirectory);

        foreach (PlanItem item in planItems)
        {
            string resolvedDestination = _conflictResolver.ResolveDestination(item.DestinationPath, conflictPolicy);

            if (string.IsNullOrWhiteSpace(resolvedDestination))
            {
                skipped++;
                runLog.Entries.Add(new RunLogEntry
                {
                    ActionKind = item.ActionKind,
                    SourcePath = item.SourcePath,
                    DestinationPath = item.DestinationPath,
                    Success = false,
                    Message = "Skipped due to conflict policy."
                });
                continue;
            }

            if (PathsEqual(item.SourcePath, resolvedDestination))
            {
                skipped++;
                runLog.Entries.Add(new RunLogEntry
                {
                    ActionKind = item.ActionKind,
                    SourcePath = item.SourcePath,
                    DestinationPath = resolvedDestination,
                    Success = false,
                    Message = "Skipped because source and destination are identical."
                });
                continue;
            }

            if (ensureDirectory != null)
            {
                ActionResult ensureResult = ensureDirectory.Execute(item.SourcePath, resolvedDestination, conflictPolicy, _fileSystem);
                if (!ensureResult.Success)
                {
                    failed++;
                    runLog.Entries.Add(new RunLogEntry
                    {
                        ActionKind = ActionKind.EnsureDirectory,
                        SourcePath = item.SourcePath,
                        DestinationPath = resolvedDestination,
                        Success = false,
                        Message = ensureResult.Message
                    });
                    continue;
                }
            }

            IFileAction action = GetAction(item.ActionKind);
            if (action == null)
            {
                failed++;
                runLog.Entries.Add(new RunLogEntry
                {
                    ActionKind = item.ActionKind,
                    SourcePath = item.SourcePath,
                    DestinationPath = resolvedDestination,
                    Success = false,
                    Message = "No action handler available."
                });
                continue;
            }

            ActionResult result = action.Execute(item.SourcePath, resolvedDestination, conflictPolicy, _fileSystem);

            if (result.Success)
            {
                succeeded++;
            }
            else
            {
                failed++;
            }

            runLog.Entries.Add(new RunLogEntry
            {
                ActionKind = item.ActionKind,
                SourcePath = item.SourcePath,
                DestinationPath = resolvedDestination,
                Success = result.Success,
                Message = result.Message
            });
        }

        return new OrganizerResult(runLog, succeeded, failed, skipped, warnings);
    }

    private IFileAction GetAction(ActionKind kind)
    {
        return _actions.FirstOrDefault(action => action.CanHandle(kind));
    }

    private bool PathsEqual(string left, string right)
    {
        StringComparison comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(left, right, comparison);
    }
}
