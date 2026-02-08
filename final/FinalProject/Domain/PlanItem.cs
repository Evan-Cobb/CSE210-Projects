namespace FinalProject.Domain;

public sealed class PlanItem
{
    public string SourcePath { get; }
    public string DestinationPath { get; }
    public ActionKind ActionKind { get; }
    public string RuleName { get; }

    public PlanItem(string sourcePath, string destinationPath, ActionKind actionKind, string ruleName)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            throw new ArgumentException("Source path is required.", nameof(sourcePath));
        }

        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentException("Destination path is required.", nameof(destinationPath));
        }

        SourcePath = sourcePath;
        DestinationPath = destinationPath;
        ActionKind = actionKind;
        RuleName = string.IsNullOrWhiteSpace(ruleName) ? "UnnamedRule" : ruleName.Trim();
    }
}
