using FinalProject.Core;

namespace FinalProject.Abilities;

public abstract class AbilityBase
{
    public string Name { get; }

    protected AbilityBase(string name)
    {
        Name = name;
    }

    public abstract void Use(GameState state);
}
