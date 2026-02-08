using FinalProject.Domain;

namespace FinalProject.Actions;

public sealed class MoveFileAction : ActionBase
{
    public override ActionKind Kind => ActionKind.MoveFile;

    public override ActionResult Execute(string sourcePath, string destinationPath, ConflictPolicy policy, IFileSystem fileSystem)
    {
        try
        {
            fileSystem.MoveFile(sourcePath, destinationPath, policy == ConflictPolicy.Overwrite);
            return ActionResult.Ok();
        }
        catch (Exception ex)
        {
            return ActionResult.Fail($"Move failed: {ex.Message}");
        }
    }
}
