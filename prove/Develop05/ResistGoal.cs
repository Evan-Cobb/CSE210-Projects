using System;

class ResistGoal : Goal
{
    private int _timesResisted;

    public ResistGoal(string name, string description, int points, int timesResisted = 0)
        : base(name, description, points)
    {
        _timesResisted = timesResisted;
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override int RecordEvent()
    {
        _timesResisted++;
        return GetPoints();
    }

    public override string GetDetailsString()
    {
        return $"[ ] {GetName()} ({GetDescription()}) -- Resisted {_timesResisted} times";
    }

    public override string Serialize()
    {
        return $"Resist|{GetName()}|{GetDescription()}|{GetPoints()}|{_timesResisted}";
    }
}
