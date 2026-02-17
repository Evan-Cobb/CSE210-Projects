using FinalProject.World;

namespace FinalProject.Rules;

public class FallbackRule : RuleBase
{
    public string Destination { get; }

    public FallbackRule(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("Destination cannot be empty.", nameof(destination));
        }

        Destination = destination;
    }

    public override bool IsMatch(VirtualFileItem item)
    {
        return true;
    }

    public override string DestinationName(VirtualFileItem item)
    {
        return Destination;
    }

    public override string Describe()
    {
        return $"FallbackRule: -> {Destination}";
    }
}
