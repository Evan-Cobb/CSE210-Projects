using System;
using System.Collections.Generic;

public class ReflectionActivity : Activity
{
    private readonly List<string> _prompts;
    private readonly List<string> _questions;
    private readonly Random _rng;

    public ReflectionActivity()
        : base(
            "Reflection",
            "This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.")
    {
        _rng = new Random();

        _prompts = new List<string>
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        _questions = new List<string>
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you get started?",
            "How did you feel when it was complete?",
            "What made this time different than other times when you were not as successful?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };
    }

    protected override void PerformActivity()
    {
        int duration = GetDurationSeconds();
        int startMs = Environment.TickCount;

        string prompt = _prompts[_rng.Next(_prompts.Count)];
        Console.WriteLine("Consider the following prompt:");
        Console.WriteLine();
        Console.WriteLine($"--- {prompt} ---");
        Console.WriteLine();
        Console.WriteLine("When you have something in mind, press Enter to continue.");
        Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Now ponder on each of the following questions as they relate to this experience.");
        Console.WriteLine("You may begin in...");
        PauseWithCountdown(3);
        Console.WriteLine();

        while (ElapsedSeconds(startMs) < duration)
        {
            string question = _questions[_rng.Next(_questions.Count)];
            Console.Write($"> {question} ");
            PauseWithSpinner(4);
            Console.WriteLine();
        }
    }

    private static int ElapsedSeconds(int startMs)
    {
        int deltaMs = Environment.TickCount - startMs;
        if (deltaMs < 0) deltaMs = int.MaxValue + deltaMs; // very unlikely edge
        return deltaMs / 1000;
    }
}
