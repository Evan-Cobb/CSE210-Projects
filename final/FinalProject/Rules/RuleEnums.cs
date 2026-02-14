namespace FinalProject.Rules;

public enum NameMatchType
{
    Contains = 1,
    StartsWith = 2,
    EndsWith = 3
}

public enum DateBucketType
{
    Year = 1,
    YearMonth = 2
}

public enum DateField
{
    CreatedUtc = 1,
    ModifiedUtc = 2
}
