namespace FinalProject.Domain;

public abstract class ActionBase : IFileAction
{
    public abstract ActionKind Kind { get; }

    public bool CanHandle(ActionKind kind) => kind == Kind;

    public abstract ActionResult Execute(string sourcePath, string destinationPath, ConflictPolicy policy, IFileSystem fileSystem);
}
