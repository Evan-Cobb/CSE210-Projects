using System;

class ChecklistGoal : Goal
{
    private int _completed;
    private int _target;
    private int _bonus;

    public ChecklistGoal(string name, string description, int points, int target, int bonus, int completed = 0)
        : base(name, description, points)
    {
        _target = target;
        _bonus = bonus;
        _completed = completed;
    }

    public override bool IsComplete()
    {
        return _completed >= _target;
    }

    public override int RecordEvent()
    {
        if (_completed >= _target)
        {
            return 0;
        }

        _completed++;
        int earned = GetPoints();
        if (_completed == _target)
        {
            earned += _bonus;
        }

        return earned;
    }

    public override string GetDetailsString()
    {
        string checkbox = IsComplete() ? "[X]" : "[ ]";
        return $"{checkbox} {GetName()} ({GetDescription()}) -- Completed {_completed}/{_target} times";
    }

    public override string Serialize()
    {
        return $"Checklist|{GetName()}|{GetDescription()}|{GetPoints()}|{_bonus}|{_target}|{_completed}";
    }
}
