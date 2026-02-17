using FinalProject.World;

namespace FinalProject.Rules;

public class DateBucketRule : RuleBase
{
    public DateField Field { get; }
    public DateBucketType BucketType { get; }
    public int Year { get; }
    public int Month { get; }
    public string Destination { get; }

    public DateBucketRule(DateField field, DateBucketType bucketType, int year, int month, string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("Destination cannot be empty.", nameof(destination));
        }
        if (year < 1 || year > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1 and 9999.");
        }
        if (bucketType == DateBucketType.YearMonth && (month < 1 || month > 12))
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12 for YearMonth.");
        }

        Field = field;
        BucketType = bucketType;
        Year = year;
        Month = month;
        Destination = destination;
    }

    public override bool IsMatch(VirtualFileItem item)
    {
        DateTime date = Field == DateField.CreatedUtc ? item.CreatedUtc : item.ModifiedUtc;
        return BucketType switch
        {
            DateBucketType.Year => date.Year == Year,
            DateBucketType.YearMonth => date.Year == Year && date.Month == Month,
            _ => false
        };
    }

    public override string DestinationName(VirtualFileItem item)
    {
        return Destination;
    }

    public override string Describe()
    {
        if (BucketType == DateBucketType.Year)
        {
            return $"DateBucketRule: {Field} Year {Year} -> {Destination}";
        }
        return $"DateBucketRule: {Field} {Year:D4}-{Month:D2} -> {Destination}";
    }
}
