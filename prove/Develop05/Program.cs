using System;

class Program
{
    // Exceeding requirements: Added avatar leveling, resist goals, and daily/weekly summaries via an activity log.
    static void Main(string[] args)
    {
        GoalManager manager = new GoalManager();
        manager.Start();
    }
}
