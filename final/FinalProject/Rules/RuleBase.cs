using FinalProject.World;

namespace FinalProject.Rules;

public abstract class RuleBase
{
    public abstract bool IsMatch(VirtualFileItem item);
    public abstract string DestinationName(VirtualFileItem item);
    public abstract string Describe();
}
