using System;
using System.Collections.Generic;

public class CustomActivity : Activity
{
    private readonly List<string> _customPrompts;
    private readonly List<string> _customResponses;

    public CustomActivity()
        : base(
            "Custom",
            "This activity allows you to create your own mindfulness activity with your own prompts and responses.")
    {
        _customPrompts = new List<string>();
        _customResponses = new List<string>();
    }

    public void AddPrompt(string prompt)
    {
        if (!string.IsNullOrWhiteSpace(prompt))
            _customPrompts.Add(prompt);
    }

    public void AddResponse(string response)
    {
        if (!string.IsNullOrWhiteSpace(response))
            _customResponses.Add(response);
    }

    protected override void PerformActivity()
    {
        _customPrompts.Clear();
        _customResponses.Clear();

        Console.WriteLine("Let's build your custom activity.");
        Console.Write("Enter a prompt for your activity: ");
        string? prompt = Console.ReadLine();
        AddPrompt(prompt ?? "");

        Console.WriteLine();
        Console.WriteLine("When you are ready, press Enter to begin.");
        Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Respond to your prompt as many times as you can:");
        Console.WriteLine();
        Console.WriteLine($"--- {(_customPrompts.Count > 0 ? _customPrompts[0] : "Your prompt") } ---");
        Console.WriteLine();

        int duration = GetDurationSeconds();
        int startMs = Environment.TickCount;

        while (ElapsedSeconds(startMs) < duration)
        {
            Console.Write("> ");
            string? resp = Console.ReadLine();
            AddResponse(resp ?? "");
        }

        Console.WriteLine();
        Console.WriteLine($"You entered {_customResponses.Count} responses.");
    }

    private static int ElapsedSeconds(int startMs)
    {
        int deltaMs = Environment.TickCount - startMs;
        if (deltaMs < 0) deltaMs = int.MaxValue + deltaMs;
        return deltaMs / 1000;
    }
}
