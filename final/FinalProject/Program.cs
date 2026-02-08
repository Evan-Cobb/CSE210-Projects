using FinalProject.Actions;
using FinalProject.Domain;
using FinalProject.Infrastructure;
using FinalProject.Services;

namespace FinalProject;

internal sealed class Program
{
    private static int Main(string[] args)
    {
        CliParseResult parseResult = CliParser.Parse(args);
        if (!parseResult.Success)
        {
            Console.WriteLine(parseResult.ErrorMessage);
            PrintUsage();
            return 1;
        }

        CliOptions options = parseResult.Options;
        string rootPath = ResolveRoot(options);

        if (string.IsNullOrWhiteSpace(rootPath))
        {
            Console.WriteLine("Root path could not be resolved.");
            PrintUsage();
            return 1;
        }

        var fileSystem = new RealFileSystem();
        string rulesPath = string.IsNullOrWhiteSpace(options.RulesPath)
            ? Path.Combine(rootPath, "rulepack.json")
            : options.RulesPath;
        string runLogPath = Path.Combine(rootPath, "runlog.json");

        if (options.Undo)
        {
            var undoService = new UndoService(fileSystem);
            UndoResult undoResult = undoService.UndoLastRun(runLogPath);
            PrintUndoResult(runLogPath, undoResult);
            return 0;
        }

        var rulePackStore = new JsonRulePackStore(fileSystem);
        RulePack rulePack = rulePackStore.LoadOrCreateDefault(rulesPath, DefaultRulePackFactory.Create, out bool createdDefault);

        if (createdDefault)
        {
            Console.WriteLine($"Default rule pack created at {rulesPath}");
        }

        var validator = new RulePackValidator();
        ValidationResult packValidation = validator.Validate(rulePack);
        if (!packValidation.IsValid)
        {
            Console.WriteLine("Rule pack validation failed:");
            foreach (string error in packValidation.Errors)
            {
                Console.WriteLine($"- {error}");
            }
            return 1;
        }

        ValidationResult rootValidation = validator.ValidateRootPath(rootPath, rulePack.BlockedDirectories, fileSystem);
        if (!rootValidation.IsValid)
        {
            Console.WriteLine("Root path validation failed:");
            foreach (string error in rootValidation.Errors)
            {
                Console.WriteLine($"- {error}");
            }
            return 1;
        }

        var ruleFactory = new RuleFactory();
        List<IFileRule> rules = ruleFactory.CreateRules(rulePack);

        var organizer = new OrganizerService(
            fileSystem,
            new PlanBuilder(fileSystem),
            new IFileAction[]
            {
                new EnsureDirectoryAction(),
                new MoveFileAction(),
                new CopyFileAction()
            });

        PlanBuildResult planResult = organizer.BuildPlan(rootPath, options.Recursive, rulePack, rules);

        if (planResult.Warnings.Count > 0)
        {
            Console.WriteLine("Warnings during scan:");
            foreach (string warning in planResult.Warnings)
            {
                Console.WriteLine($"- {warning}");
            }
        }

        if (options.Mode == OrganizeMode.DryRun)
        {
            PrintPlanSummary(planResult.Items);
            return 0;
        }

        OrganizerResult executeResult = organizer.ExecutePlan(planResult.Items, options.ConflictPolicy, rootPath);
        var runLogStore = new JsonRunLogStore(fileSystem);
        runLogStore.Save(runLogPath, executeResult.RunLog);

        PrintExecutionSummary(runLogPath, executeResult);
        return 0;
    }

    private static string ResolveRoot(CliOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.RootPath))
        {
            return options.RootPath;
        }

        if (string.IsNullOrWhiteSpace(options.Preset))
        {
            return string.Empty;
        }

        string preset = options.Preset.Trim().ToLowerInvariant();
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return preset switch
        {
            "downloads" => ResolveDownloadsPath(userProfile),
            "pictures" => ResolveSpecialFolder(Environment.SpecialFolder.MyPictures, userProfile, "Pictures"),
            "documents" => ResolveSpecialFolder(Environment.SpecialFolder.MyDocuments, userProfile, "Documents"),
            _ => string.Empty
        };
    }

    private static string ResolveDownloadsPath(string userProfile)
    {
        if (string.IsNullOrWhiteSpace(userProfile))
        {
            return string.Empty;
        }

        return Path.Combine(userProfile, "Downloads");
    }

    private static string ResolveSpecialFolder(Environment.SpecialFolder folder, string userProfile, string fallbackName)
    {
        string path = Environment.GetFolderPath(folder);
        if (!string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        if (string.IsNullOrWhiteSpace(userProfile))
        {
            return string.Empty;
        }

        return Path.Combine(userProfile, fallbackName);
    }

    private static void PrintPlanSummary(List<PlanItem> plan)
    {
        Console.WriteLine("Dry run plan summary:");
        Console.WriteLine($"Total files matched: {plan.Count}");

        if (plan.Count == 0)
        {
            return;
        }

        Console.WriteLine("Destinations:");
        var grouped = plan
            .GroupBy(item => Path.GetDirectoryName(item.DestinationPath) ?? string.Empty)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var group in grouped)
        {
            Console.WriteLine($"- {group.Key} : {group.Count()}");
        }

        Console.WriteLine("Sample moves:");
        foreach (PlanItem item in plan.Take(5))
        {
            Console.WriteLine($"- {item.SourcePath} -> {item.DestinationPath}");
        }
    }

    private static void PrintExecutionSummary(string runLogPath, OrganizerResult result)
    {
        Console.WriteLine("Apply mode complete:");
        Console.WriteLine($"Succeeded: {result.Succeeded}");
        Console.WriteLine($"Failed: {result.Failed}");
        Console.WriteLine($"Skipped: {result.Skipped}");
        Console.WriteLine($"Run log: {runLogPath}");
    }

    private static void PrintUndoResult(string runLogPath, UndoResult result)
    {
        Console.WriteLine("Undo complete:");
        Console.WriteLine($"Reverted: {result.Reverted}");
        Console.WriteLine($"Skipped: {result.Skipped}");
        Console.WriteLine($"Failed: {result.Failed}");
        Console.WriteLine($"Run log: {runLogPath}");

        if (result.Warnings.Count > 0)
        {
            Console.WriteLine("Warnings:");
            foreach (string warning in result.Warnings)
            {
                Console.WriteLine($"- {warning}");
            }
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  --preset downloads|pictures|documents OR --root <path>");
        Console.WriteLine("  --recursive (optional)");
        Console.WriteLine("  --mode dryrun|apply (default dryrun)");
        Console.WriteLine("  --rules <path> (optional; default is <root>/rulepack.json)");
        Console.WriteLine("  --undo (undo last run; ignores organize mode)");
        Console.WriteLine("  --conflict skip|rename|overwrite (default rename)");
    }
}

internal sealed class CliOptions
{
    public string Preset { get; init; } = string.Empty;
    public string RootPath { get; init; } = string.Empty;
    public bool Recursive { get; init; }
    public OrganizeMode Mode { get; init; } = OrganizeMode.DryRun;
    public string RulesPath { get; init; } = string.Empty;
    public bool Undo { get; init; }
    public ConflictPolicy ConflictPolicy { get; init; } = ConflictPolicy.RenameWithCounter;
}

internal sealed class CliParseResult
{
    public bool Success { get; }
    public CliOptions Options { get; }
    public string ErrorMessage { get; }

    public CliParseResult(bool success, CliOptions options, string errorMessage)
    {
        Success = success;
        Options = options;
        ErrorMessage = errorMessage;
    }
}

internal static class CliParser
{
    public static CliParseResult Parse(string[] args)
    {
        string preset = string.Empty;
        string rootPath = string.Empty;
        bool recursive = false;
        OrganizeMode mode = OrganizeMode.DryRun;
        string rulesPath = string.Empty;
        bool undo = false;
        ConflictPolicy conflictPolicy = ConflictPolicy.RenameWithCounter;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            switch (arg)
            {
                case "--preset":
                    if (!TryReadValue(args, ref i, out preset))
                    {
                        return new CliParseResult(false, new CliOptions(), "Missing value for --preset.");
                    }
                    break;
                case "--root":
                    if (!TryReadValue(args, ref i, out rootPath))
                    {
                        return new CliParseResult(false, new CliOptions(), "Missing value for --root.");
                    }
                    break;
                case "--recursive":
                    recursive = true;
                    break;
                case "--mode":
                    if (!TryReadValue(args, ref i, out string modeValue))
                    {
                        return new CliParseResult(false, new CliOptions(), "Missing value for --mode.");
                    }
                    mode = ParseMode(modeValue, out string modeError);
                    if (!string.IsNullOrWhiteSpace(modeError))
                    {
                        return new CliParseResult(false, new CliOptions(), modeError);
                    }
                    break;
                case "--rules":
                    if (!TryReadValue(args, ref i, out rulesPath))
                    {
                        return new CliParseResult(false, new CliOptions(), "Missing value for --rules.");
                    }
                    break;
                case "--undo":
                    undo = true;
                    break;
                case "--conflict":
                    if (!TryReadValue(args, ref i, out string conflictValue))
                    {
                        return new CliParseResult(false, new CliOptions(), "Missing value for --conflict.");
                    }
                    conflictPolicy = ParseConflict(conflictValue, out string conflictError);
                    if (!string.IsNullOrWhiteSpace(conflictError))
                    {
                        return new CliParseResult(false, new CliOptions(), conflictError);
                    }
                    break;
                default:
                    return new CliParseResult(false, new CliOptions(), $"Unknown argument: {arg}");
            }
        }

        if (!string.IsNullOrWhiteSpace(preset) && !string.IsNullOrWhiteSpace(rootPath))
        {
            return new CliParseResult(false, new CliOptions(), "Use either --preset or --root, not both.");
        }

        if (string.IsNullOrWhiteSpace(preset) && string.IsNullOrWhiteSpace(rootPath))
        {
            return new CliParseResult(false, new CliOptions(), "You must supply --preset or --root.");
        }

        var options = new CliOptions
        {
            Preset = preset,
            RootPath = rootPath,
            Recursive = recursive,
            Mode = mode,
            RulesPath = rulesPath,
            Undo = undo,
            ConflictPolicy = conflictPolicy
        };

        return new CliParseResult(true, options, string.Empty);
    }

    private static bool TryReadValue(string[] args, ref int index, out string value)
    {
        if (index + 1 >= args.Length)
        {
            value = string.Empty;
            return false;
        }

        value = args[index + 1];
        index++;
        return true;
    }

    private static OrganizeMode ParseMode(string value, out string error)
    {
        error = string.Empty;
        string normalized = value.Trim().ToLowerInvariant();
        return normalized switch
        {
            "dryrun" => OrganizeMode.DryRun,
            "apply" => OrganizeMode.Apply,
            _ => SetModeError(value, out error)
        };
    }

    private static OrganizeMode SetModeError(string value, out string error)
    {
        error = $"Invalid --mode value: {value}";
        return OrganizeMode.DryRun;
    }

    private static ConflictPolicy ParseConflict(string value, out string error)
    {
        error = string.Empty;
        string normalized = value.Trim().ToLowerInvariant();
        return normalized switch
        {
            "skip" => ConflictPolicy.Skip,
            "rename" => ConflictPolicy.RenameWithCounter,
            "overwrite" => ConflictPolicy.Overwrite,
            _ => SetConflictError(value, out error)
        };
    }

    private static ConflictPolicy SetConflictError(string value, out string error)
    {
        error = $"Invalid --conflict value: {value}";
        return ConflictPolicy.RenameWithCounter;
    }
}
