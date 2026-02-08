using FinalProject.Domain;

namespace FinalProject.Services;

public sealed class ConflictResolver
{
    private readonly IFileSystem _fileSystem;

    public ConflictResolver(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public string ResolveDestination(string destinationPath, ConflictPolicy policy)
    {
        if (!_fileSystem.FileExists(destinationPath))
        {
            return destinationPath;
        }

        if (policy == ConflictPolicy.Skip)
        {
            return null;
        }

        if (policy == ConflictPolicy.Overwrite)
        {
            return destinationPath;
        }

        return GetNextAvailableName(destinationPath);
    }

    private string GetNextAvailableName(string destinationPath)
    {
        string directory = _fileSystem.GetDirectoryName(destinationPath);
        string fileName = _fileSystem.GetFileNameWithoutExtension(destinationPath);
        string extension = _fileSystem.GetExtension(destinationPath);

        for (int i = 1; i < 10000; i++)
        {
            string candidateName = $"{fileName} ({i}){extension}";
            string candidatePath = Path.Combine(directory, candidateName);
            if (!_fileSystem.FileExists(candidatePath))
            {
                return candidatePath;
            }
        }

        return destinationPath;
    }
}
