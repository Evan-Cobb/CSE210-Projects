using System;
using System.Collections.Generic;
using System.IO;

class GoalManager
{
    private List<Goal> _goals;
    private int _score;
    private Avatar _avatar;
    private ActivityLog _log;

    public GoalManager()
    {
        _goals = new List<Goal>();
        _score = 0;
        _avatar = null;
        _log = new ActivityLog();
    }

    public void Start()
    {
        if (_avatar == null)
        {
            Console.Write("Enter your avatar name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Adventurer";
            }

            _avatar = new Avatar(name);
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine();
            DisplayScore();
            Console.WriteLine("Menu Options:");
            Console.WriteLine("1. Create New Goal");
            Console.WriteLine("2. List Goals");
            Console.WriteLine("3. Save Goals");
            Console.WriteLine("4. Load Goals");
            Console.WriteLine("5. Record Event");
            Console.WriteLine("6. Show Daily Summary");
            Console.WriteLine("7. Show Weekly Summary");
            Console.WriteLine("8. Quit");
            Console.Write("Select a choice from the menu: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateGoal();
                    break;
                case "2":
                    ListGoals();
                    break;
                case "3":
                    Save(PromptString("Enter filename to save: "));
                    break;
                case "4":
                    Load(PromptString("Enter filename to load: "));
                    break;
                case "5":
                    RecordEvent();
                    break;
                case "6":
                    ShowDailySummary();
                    break;
                case "7":
                    ShowWeeklySummary();
                    break;
                case "8":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    public void DisplayScore()
    {
        Console.WriteLine($"Score: {_score}");
        Console.WriteLine(_avatar.GetStatus());
    }

    public void ListGoals()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals yet.");
            return;
        }

        Console.WriteLine("Your Goals:");
        for (int i = 0; i < _goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_goals[i].GetDetailsString()}");
        }
    }

    public void CreateGoal()
    {
        Console.WriteLine("The types of Goals are:");
        Console.WriteLine("1. Simple Goal");
        Console.WriteLine("2. Eternal Goal");
        Console.WriteLine("3. Checklist Goal");
        Console.WriteLine("4. Resist Goal");
        Console.Write("Which type of goal would you like to create? ");
        string choice = Console.ReadLine();

        string name = PromptString("What is the name of your goal? ");
        string description = PromptString("What is a short description of it? ");
        int points = PromptInt("What is the amount of points associated with this goal? ");

        switch (choice)
        {
            case "1":
                _goals.Add(new SimpleGoal(name, description, points));
                break;
            case "2":
                _goals.Add(new EternalGoal(name, description, points));
                break;
            case "3":
                int target = PromptInt("How many times does this goal need to be accomplished for a bonus? ");
                int bonus = PromptInt("What is the bonus for accomplishing it that many times? ");
                _goals.Add(new ChecklistGoal(name, description, points, target, bonus));
                break;
            case "4":
                _goals.Add(new ResistGoal(name, description, points));
                break;
            default:
                Console.WriteLine("Invalid goal type.");
                break;
        }
    }

    public void RecordEvent()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals to record.");
            return;
        }

        Console.WriteLine("Select a goal to record:");
        for (int i = 0; i < _goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_goals[i].GetDetailsString()}");
        }

        int choice = PromptInt("Which goal did you accomplish? ") - 1;
        if (choice < 0 || choice >= _goals.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        Goal goal = _goals[choice];
        int pointsEarned = goal.RecordEvent();
        if (pointsEarned <= 0)
        {
            Console.WriteLine("No points earned for that event.");
            return;
        }

        _score += pointsEarned;
        _avatar.AddXp(pointsEarned);
        _log.Add(new LogEntry(DateTime.Now, goal.GetName(), pointsEarned));

        Console.WriteLine($"You earned {pointsEarned} points.");
    }

    public void ShowDailySummary()
    {
        DateTime today = DateTime.Today;
        List<LogEntry> entries = _log.GetDaily(today);

        Console.WriteLine($"Daily Summary ({today:yyyy-MM-dd})");
        if (entries.Count == 0)
        {
            Console.WriteLine("No entries for today.");
            return;
        }

        int total = 0;
        foreach (LogEntry entry in entries)
        {
            total += entry.Points;
            Console.WriteLine($"{entry.Date:HH:mm} - {entry.GoalName} (+{entry.Points})");
        }

        Console.WriteLine($"Total: {total} points");
    }

    public void ShowWeeklySummary()
    {
        DateTime today = DateTime.Today;
        int diff = ((int)today.DayOfWeek + 6) % 7;
        DateTime weekStart = today.AddDays(-diff);
        DateTime weekEnd = weekStart.AddDays(6);

        List<LogEntry> entries = _log.GetWeekly(weekStart);

        Console.WriteLine($"Weekly Summary ({weekStart:yyyy-MM-dd} to {weekEnd:yyyy-MM-dd})");
        if (entries.Count == 0)
        {
            Console.WriteLine("No entries for this week.");
            return;
        }

        int total = 0;
        foreach (LogEntry entry in entries)
        {
            total += entry.Points;
            Console.WriteLine($"{entry.Date:yyyy-MM-dd HH:mm} - {entry.GoalName} (+{entry.Points})");
        }

        Console.WriteLine($"Total: {total} points");
    }

    public void Save(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            Console.WriteLine("Save canceled.");
            return;
        }

        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine($"SCORE|{_score}");
            writer.WriteLine($"AVATAR|{_avatar.GetName()}|{_avatar.GetLevel()}|{_avatar.GetXp()}");
            writer.WriteLine($"GOALS|{_goals.Count}");

            foreach (Goal goal in _goals)
            {
                writer.WriteLine(goal.Serialize());
            }
        }

        Console.WriteLine("Goals saved.");
    }

    public void Load(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            Console.WriteLine("Load canceled.");
            return;
        }

        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found.");
            return;
        }

        string[] lines = File.ReadAllLines(filename);
        if (lines.Length == 0)
        {
            Console.WriteLine("File is empty.");
            return;
        }

        _goals.Clear();
        _score = 0;
        _log = new ActivityLog();

        int index = 0;

        if (index < lines.Length && lines[index].StartsWith("SCORE|"))
        {
            string[] parts = lines[index].Split('|');
            if (parts.Length > 1)
            {
                int.TryParse(parts[1], out _score);
            }
            index++;
        }

        if (index < lines.Length && lines[index].StartsWith("AVATAR|"))
        {
            string[] parts = lines[index].Split('|');
            string name = parts.Length > 1 ? parts[1] : "Adventurer";
            int level = 1;
            int xp = 0;

            if (parts.Length > 2)
            {
                int.TryParse(parts[2], out level);
            }
            if (parts.Length > 3)
            {
                int.TryParse(parts[3], out xp);
            }

            _avatar = new Avatar(name, level, xp);
            index++;
        }

        if (index < lines.Length && lines[index].StartsWith("GOALS|"))
        {
            string[] parts = lines[index].Split('|');
            int count = 0;
            if (parts.Length > 1)
            {
                int.TryParse(parts[1], out count);
            }
            index++;

            for (int i = 0; i < count && index < lines.Length; i++, index++)
            {
                Goal goal = DeserializeGoal(lines[index]);
                if (goal != null)
                {
                    _goals.Add(goal);
                }
            }
        }

        if (_avatar == null)
        {
            _avatar = new Avatar("Adventurer");
        }

        Console.WriteLine("Goals loaded.");
    }

    private Goal DeserializeGoal(string line)
    {
        string[] parts = line.Split('|');
        if (parts.Length == 0)
        {
            return null;
        }

        string type = parts[0];
        if (type == "Simple" && parts.Length >= 5)
        {
            string name = parts[1];
            string description = parts[2];
            int points = int.Parse(parts[3]);
            bool isComplete = bool.Parse(parts[4]);
            return new SimpleGoal(name, description, points, isComplete);
        }
        if (type == "Eternal" && parts.Length >= 4)
        {
            string name = parts[1];
            string description = parts[2];
            int points = int.Parse(parts[3]);
            return new EternalGoal(name, description, points);
        }
        if (type == "Checklist" && parts.Length >= 7)
        {
            string name = parts[1];
            string description = parts[2];
            int points = int.Parse(parts[3]);
            int bonus = int.Parse(parts[4]);
            int target = int.Parse(parts[5]);
            int completed = int.Parse(parts[6]);
            return new ChecklistGoal(name, description, points, target, bonus, completed);
        }
        if (type == "Resist" && parts.Length >= 5)
        {
            string name = parts[1];
            string description = parts[2];
            int points = int.Parse(parts[3]);
            int timesResisted = int.Parse(parts[4]);
            return new ResistGoal(name, description, points, timesResisted);
        }

        return null;
    }

    private string PromptString(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        return input ?? string.Empty;
    }

    private int PromptInt(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        int value;

        while (!int.TryParse(input, out value))
        {
            Console.Write("Please enter a whole number: ");
            input = Console.ReadLine();
        }

        return value;
    }
}
