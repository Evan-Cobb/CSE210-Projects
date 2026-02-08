using FinalProject.Domain;

namespace FinalProject.Infrastructure;

public sealed class RealFileSystem : IFileSystem
{
    public IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

    public IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public bool FileExists(string path) => File.Exists(path);

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public void MoveFile(string sourcePath, string destinationPath, bool overwrite) => File.Move(sourcePath, destinationPath, overwrite);

    public void CopyFile(string sourcePath, string destinationPath, bool overwrite) => File.Copy(sourcePath, destinationPath, overwrite);

    public DateTime GetCreationTimeUtc(string path) => File.GetCreationTimeUtc(path);

    public DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);

    public string GetFileName(string path) => Path.GetFileName(path);

    public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);

    public string GetExtension(string path) => Path.GetExtension(path);

    public string GetDirectoryName(string path) => Path.GetDirectoryName(path) ?? string.Empty;

    public string GetFullPath(string path) => Path.GetFullPath(path);

    public FileAttributes GetAttributes(string path) => File.GetAttributes(path);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
}
