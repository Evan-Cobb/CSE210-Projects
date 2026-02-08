namespace FinalProject.Domain;

public interface IFileRule
{
    string Name { get; }
    int Priority { get; }
    bool Enabled { get; }
    bool TryMatch(FileItem item, out RuleMatch match);
}
