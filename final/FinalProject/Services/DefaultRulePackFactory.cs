using FinalProject.Domain;

namespace FinalProject.Services;

public static class DefaultRulePackFactory
{
    public static RulePack Create()
    {
        return new RulePack
        {
            Version = 1,
            BlockedDirectories = new List<string> { ".git", "bin", "obj" },
            IncludeExtensions = new List<string>(),
            ExcludeExtensions = new List<string>(),
            Rules = new List<RuleDefinition>
            {
                new RuleDefinition
                {
                    Type = "ExtensionRule",
                    Name = "Images",
                    Priority = 100,
                    Enabled = true,
                    Destination = "Images",
                    Action = ActionKind.MoveFile,
                    Extensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" }
                },
                new RuleDefinition
                {
                    Type = "ExtensionRule",
                    Name = "Documents",
                    Priority = 90,
                    Enabled = true,
                    Destination = "Documents",
                    Action = ActionKind.MoveFile,
                    Extensions = new List<string> { ".pdf", ".doc", ".docx", ".txt", ".md", ".rtf", ".xls", ".xlsx", ".ppt", ".pptx" }
                },
                new RuleDefinition
                {
                    Type = "ExtensionRule",
                    Name = "Archives",
                    Priority = 80,
                    Enabled = true,
                    Destination = "Archives",
                    Action = ActionKind.MoveFile,
                    Extensions = new List<string> { ".zip", ".7z", ".rar", ".tar", ".gz" }
                },
                new RuleDefinition
                {
                    Type = "NamePatternRule",
                    Name = "Screenshots",
                    Priority = 70,
                    Enabled = true,
                    Destination = "Screenshots",
                    Action = ActionKind.MoveFile,
                    Pattern = "Screenshot",
                    Match = PatternMatchType.Contains
                },
                new RuleDefinition
                {
                    Type = "DateBucketRule",
                    Name = "ByDate",
                    Priority = 10,
                    Enabled = true,
                    Destination = "ByDate",
                    Action = ActionKind.MoveFile,
                    Bucket = DateBucket.YearMonth,
                    DateSource = DateSource.Modified
                },
                new RuleDefinition
                {
                    Type = "FallbackRule",
                    Name = "Other",
                    Priority = 0,
                    Enabled = true,
                    Destination = "Other",
                    Action = ActionKind.MoveFile
                }
            }
        };
    }
}
