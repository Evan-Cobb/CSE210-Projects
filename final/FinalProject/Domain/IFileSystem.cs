using System.IO;

namespace FinalProject.Domain;

public interface IFileSystem
{
    IEnumerable<string> EnumerateDirectories(string path);
    IEnumerable<string> EnumerateFiles(string path);
    bool DirectoryExists(string path);
    bool FileExists(string path);
    void CreateDirectory(string path);
    void MoveFile(string sourcePath, string destinationPath, bool overwrite);
    void CopyFile(string sourcePath, string destinationPath, bool overwrite);
    DateTime GetCreationTimeUtc(string path);
    DateTime GetLastWriteTimeUtc(string path);
    string GetFileName(string path);
    string GetFileNameWithoutExtension(string path);
    string GetExtension(string path);
    string GetDirectoryName(string path);
    string GetFullPath(string path);
    FileAttributes GetAttributes(string path);
    string ReadAllText(string path);
    void WriteAllText(string path, string contents);
}
