namespace FinalProject.Core;

public record SortResult(
    string RuleDescription,
    string Destination,
    string CorrectDestination,
    bool IsCorrect,
    int TurnCost);
