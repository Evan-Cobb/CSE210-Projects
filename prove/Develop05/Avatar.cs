using System;

class Avatar
{
    private string _name;
    private int _level;
    private int _xp;

    public Avatar(string name)
        : this(name, 1, 0)
    {
    }

    public Avatar(string name, int level, int xp)
    {
        _name = string.IsNullOrWhiteSpace(name) ? "Adventurer" : name;
        _level = level < 1 ? 1 : level;
        _xp = xp < 0 ? 0 : xp;
    }

    public void AddXp(int points)
    {
        if (points <= 0)
        {
            return;
        }

        _xp += points;
        while (_xp >= GetLevelThreshold())
        {
            _xp -= GetLevelThreshold();
            _level++;
        }
    }

    public string GetStatus()
    {
        return $"Avatar: {_name} | Level {_level} | XP {_xp}/{GetLevelThreshold()}";
    }

    public string GetName()
    {
        return _name;
    }

    public int GetLevel()
    {
        return _level;
    }

    public int GetXp()
    {
        return _xp;
    }

    private int GetLevelThreshold()
    {
        return 100;
    }
}
