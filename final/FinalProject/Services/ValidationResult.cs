namespace FinalProject.Services;

public sealed class ValidationResult
{
    public List<string> Errors { get; } = new List<string>();
    public List<string> Warnings { get; } = new List<string>();
    public bool IsValid => Errors.Count == 0;
}
