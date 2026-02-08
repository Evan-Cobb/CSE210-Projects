using FinalProject.Domain;

namespace FinalProject.Rules;

public sealed class DateBucketRule : RuleBase
{
    private readonly DateBucket _bucket;
    private readonly DateSource _source;

    public DateBucketRule(string name, int priority, bool enabled, string destination, ActionKind actionKind, DateBucket bucket, DateSource source)
        : base(name, priority, enabled, destination, actionKind)
    {
        _bucket = bucket;
        _source = source;
    }

    protected override bool IsMatch(FileItem item, out string destinationSubPath)
    {
        DateTime date = _source == DateSource.Created ? item.CreatedUtc : item.ModifiedUtc;
        string bucketValue = _bucket == DateBucket.Year
            ? date.ToString("yyyy")
            : date.ToString("yyyy-MM");

        destinationSubPath = string.IsNullOrWhiteSpace(Destination)
            ? bucketValue
            : Path.Combine(Destination, bucketValue);

        return true;
    }
}
