using System;
using System.Collections.Generic;

class ActivityLog
{
    private List<LogEntry> _entries;

    public ActivityLog()
    {
        _entries = new List<LogEntry>();
    }

    public void Add(LogEntry entry)
    {
        _entries.Add(entry);
    }

    public List<LogEntry> GetDaily(DateTime day)
    {
        List<LogEntry> results = new List<LogEntry>();
        DateTime target = day.Date;

        foreach (LogEntry entry in _entries)
        {
            if (entry.Date.Date == target)
            {
                results.Add(entry);
            }
        }

        return results;
    }

    public List<LogEntry> GetWeekly(DateTime weekStart)
    {
        List<LogEntry> results = new List<LogEntry>();
        DateTime start = weekStart.Date;
        DateTime end = start.AddDays(7);

        foreach (LogEntry entry in _entries)
        {
            DateTime date = entry.Date;
            if (date >= start && date < end)
            {
                results.Add(entry);
            }
        }

        return results;
    }
}
