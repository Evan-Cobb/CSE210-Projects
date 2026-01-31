using System;
using System.Collections.Generic;

public class ListingActivity : Activity
{
    private readonly List<string> _prompts;
    private readonly List<string> _responses;
    private readonly Random _rng;

    public ListingActivity()
        : base(
            "Listing",
            "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.")
    {
        _rng = new Random();

        _prompts = new List<string>
        {
            "Who are people that you appreciate?",
            "What are personal strengths of yours?",
            "Who are people that you have helped this week?",
            "When have you felt the Holy Ghost this month?",
            "Who are some of your personal heroes?"
        };

        _responses = new List<string>();
    }

    protected override void PerformActivity()
    {
        _responses.Clear();

        int duration = GetDurationSeconds();
        int startMs = Environment.TickCount;

        string prompt = _prompts[_rng.Next(_prompts.Count)];

        Console.WriteLine("List as many responses as you can to the following prompt:");
        Console.WriteLine();
        Console.WriteLine($"--- {prompt} ---");
        Console.WriteLine();
        Console.Write("You may begin in: ");
        PauseWithCountdown(3);

        Console.WriteLine();
        Console.WriteLine("Start listing items. Press Enter after each one.");

        while (ElapsedSeconds(startMs) < duration)
        {
            Console.Write("> ");
            string? item = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(item))
                _responses.Add(item);
        }

        Console.WriteLine();
        Console.WriteLine($"You listed {GetResponseCount()} items!");
    }

    public int GetResponseCount()
    {
        return _responses.Count;
    }

    private static int ElapsedSeconds(int startMs)
    {
        int deltaMs = Environment.TickCount - startMs;
        if (deltaMs < 0) deltaMs = int.MaxValue + deltaMs;
        return deltaMs / 1000;
    }
}
