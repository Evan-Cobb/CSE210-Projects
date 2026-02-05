using System;

class LogEntry
{
    private DateTime _date;
    private string _goalName;
    private int _points;

    public LogEntry(DateTime date, string goalName, int points)
    {
        _date = date;
        _goalName = goalName;
        _points = points;
    }

    public DateTime Date
    {
        get { return _date; }
    }

    public string GoalName
    {
        get { return _goalName; }
    }

    public int Points
    {
        get { return _points; }
    }
}
