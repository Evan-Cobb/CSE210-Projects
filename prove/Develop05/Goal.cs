using System;

abstract class Goal
{
    private string _name;
    private string _description;
    private int _points;

    protected Goal(string name, string description, int points)
    {
        _name = name;
        _description = description;
        _points = points;
    }

    public string GetName()
    {
        return _name;
    }

    public string GetDescription()
    {
        return _description;
    }

    protected int GetPoints()
    {
        return _points;
    }

    public abstract bool IsComplete();
    public abstract int RecordEvent();
    public abstract string GetDetailsString();
    public abstract string Serialize();
}
