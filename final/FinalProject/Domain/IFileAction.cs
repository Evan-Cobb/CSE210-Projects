namespace FinalProject.Domain;

public interface IFileAction
{
    ActionKind Kind { get; }
    bool CanHandle(ActionKind kind);
    ActionResult Execute(string sourcePath, string destinationPath, ConflictPolicy policy, IFileSystem fileSystem);
}
