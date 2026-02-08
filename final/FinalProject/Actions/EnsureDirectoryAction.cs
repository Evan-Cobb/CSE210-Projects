using FinalProject.Domain;

namespace FinalProject.Actions;

public sealed class EnsureDirectoryAction : ActionBase
{
    public override ActionKind Kind => ActionKind.EnsureDirectory;

    public override ActionResult Execute(string sourcePath, string destinationPath, ConflictPolicy policy, IFileSystem fileSystem)
    {
        try
        {
            string directory = fileSystem.GetDirectoryName(destinationPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                fileSystem.CreateDirectory(directory);
            }

            return ActionResult.Ok();
        }
        catch (Exception ex)
        {
            return ActionResult.Fail($"Ensure directory failed: {ex.Message}");
        }
    }
}
