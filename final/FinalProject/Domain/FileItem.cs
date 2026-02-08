namespace FinalProject.Domain;

public sealed class FileItem
{
    public string FullPath { get; }
    public string FileName { get; }
    public string FileNameWithoutExtension { get; }
    public string Extension { get; }
    public DateTime CreatedUtc { get; }
    public DateTime ModifiedUtc { get; }

    public FileItem(
        string fullPath,
        string fileName,
        string fileNameWithoutExtension,
        string extension,
        DateTime createdUtc,
        DateTime modifiedUtc)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            throw new ArgumentException("Full path is required.", nameof(fullPath));
        }

        FullPath = fullPath;
        FileName = fileName ?? string.Empty;
        FileNameWithoutExtension = fileNameWithoutExtension ?? string.Empty;
        Extension = extension ?? string.Empty;
        CreatedUtc = createdUtc;
        ModifiedUtc = modifiedUtc;
    }
}
