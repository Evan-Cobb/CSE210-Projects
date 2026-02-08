namespace FinalProject.Domain;

public enum ActionKind
{
    EnsureDirectory,
    MoveFile,
    CopyFile
}

public enum ConflictPolicy
{
    Skip,
    RenameWithCounter,
    Overwrite
}

public enum PatternMatchType
{
    Contains,
    StartsWith,
    EndsWith
}

public enum DateBucket
{
    Year,
    YearMonth
}

public enum DateSource
{
    Created,
    Modified
}

public enum OrganizeMode
{
    DryRun,
    Apply
}
