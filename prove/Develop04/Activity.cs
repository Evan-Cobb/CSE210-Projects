using System;
using System.Threading;

public abstract class Activity
{
    private readonly string _name;
    private readonly string _description;
    private int _durationSeconds;

    protected Activity(string name, string description)
    {
        _name = name;
        _description = description;
        _durationSeconds = 0;
    }

    public void Start()
    {
        ShowStartingMessage();
        AskAndSetDuration();
        ShowPrepareToBegin();

        PerformActivity();

        ShowEndingMessage();
    }

    private void ShowStartingMessage()
    {
        Console.Clear();
        Console.WriteLine($"Welcome to the {_name} Activity.");
        Console.WriteLine();
        Console.WriteLine(_description);
        Console.WriteLine();
    }

    private void AskAndSetDuration()
    {
        Console.Write("How long, in seconds, would you like for your session? ");

        while (true)
        {
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int seconds) && seconds > 0)
            {
                _durationSeconds = seconds;
                return;
            }

            Console.Write("Please enter a positive whole number of seconds: ");
        }
    }

    private void ShowPrepareToBegin()
    {
        Console.WriteLine();
        Console.WriteLine("Get ready...");
        PauseWithSpinner(3);
        Console.WriteLine();
    }

    private void ShowEndingMessage()
    {
        Console.WriteLine();
        Console.WriteLine("Well done!!");
        PauseWithSpinner(3);

        Console.WriteLine();
        Console.WriteLine($"You have completed another {_name} Activity.");
        Console.WriteLine($"Duration: {_durationSeconds} seconds.");
        PauseWithSpinner(3);
        Console.WriteLine();
    }

    protected void PauseWithSpinner(int seconds)
    {
        // simple spinner animation for total "seconds"
        char[] frames = new[] { '|', '/', '-', '\\' };
        int totalTicks = seconds * 10; // 10 updates per second
        int frameIndex = 0;

        for (int i = 0; i < totalTicks; i++)
        {
            Console.Write(frames[frameIndex]);
            Thread.Sleep(100);
            Console.Write('\b');

            frameIndex = (frameIndex + 1) % frames.Length;
        }
    }

    protected void PauseWithCountdown(int seconds)
    {
        for (int i = seconds; i >= 1; i--)
        {
            Console.Write(i);
            Thread.Sleep(1000);
            Console.Write('\b');

            // If we printed "10", we need to clear 2 chars. This keeps it simple:
            if (i >= 10)
                Console.Write('\b');
        }
        Console.WriteLine();
    }

    protected int GetDurationSeconds()
    {
        return _durationSeconds;
    }

    protected abstract void PerformActivity();
}
