using System;

public class BreathingActivity : Activity
{
    private readonly int _breathIntervalSeconds;

    public BreathingActivity()
        : base(
            "Breathing",
            "This activity will help you relax by walking you through breathing in and out slowly. Clear your mind and focus on your breathing.")
    {
        _breathIntervalSeconds = 3;
    }

    protected override void PerformActivity()
    {
        int duration = GetDurationSeconds();
        int elapsed = 0;

        while (elapsed < duration)
        {
            Console.WriteLine("Breathe in...");
            PauseWithCountdown(_breathIntervalSeconds);
            elapsed += _breathIntervalSeconds;
            if (elapsed >= duration) break;

            Console.WriteLine("Breathe out...");
            PauseWithCountdown(_breathIntervalSeconds);
            elapsed += _breathIntervalSeconds;
        }
    }
}
