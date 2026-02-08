using FinalProject.Domain;

namespace FinalProject.Actions;

public sealed class CopyFileAction : ActionBase
{
    public override ActionKind Kind => ActionKind.CopyFile;

    public override ActionResult Execute(string sourcePath, string destinationPath, ConflictPolicy policy, IFileSystem fileSystem)
    {
        try
        {
            fileSystem.CopyFile(sourcePath, destinationPath, policy == ConflictPolicy.Overwrite);
            return ActionResult.Ok();
        }
        catch (Exception ex)
        {
            return ActionResult.Fail($"Copy failed: {ex.Message}");
        }
    }
}
