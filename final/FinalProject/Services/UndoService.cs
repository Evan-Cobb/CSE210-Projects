using FinalProject.Domain;
using FinalProject.Infrastructure;

namespace FinalProject.Services;

public sealed class UndoService
{
    private readonly IFileSystem _fileSystem;
    private readonly JsonRunLogStore _runLogStore;

    public UndoService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _runLogStore = new JsonRunLogStore(fileSystem);
    }

    public UndoResult UndoLastRun(string runLogPath)
    {
        var warnings = new List<string>();

        if (!_fileSystem.FileExists(runLogPath))
        {
            warnings.Add($"Run log not found: {runLogPath}");
            return new UndoResult(0, 0, 0, warnings);
        }

        RunLog log;
        try
        {
            log = _runLogStore.Load(runLogPath);
        }
        catch (Exception ex)
        {
            warnings.Add($"Failed to read run log: {ex.Message}");
            return new UndoResult(0, 0, 0, warnings);
        }

        int reverted = 0;
        int skipped = 0;
        int failed = 0;

        foreach (RunLogEntry entry in log.Entries)
        {
            if (entry.ActionKind != ActionKind.MoveFile || !entry.Success)
            {
                if (entry.ActionKind == ActionKind.CopyFile && entry.Success)
                {
                    warnings.Add($"Copy action not undone: {entry.DestinationPath}");
                }
                continue;
            }

            if (!_fileSystem.FileExists(entry.DestinationPath))
            {
                skipped++;
                warnings.Add($"Missing destination file, skip undo: {entry.DestinationPath}");
                continue;
            }

            if (_fileSystem.FileExists(entry.SourcePath))
            {
                skipped++;
                warnings.Add($"Source already exists, skip undo: {entry.SourcePath}");
                continue;
            }

            try
            {
                _fileSystem.MoveFile(entry.DestinationPath, entry.SourcePath, false);
                reverted++;
            }
            catch (Exception ex)
            {
                failed++;
                warnings.Add($"Failed to undo move for '{entry.DestinationPath}': {ex.Message}");
            }
        }

        return new UndoResult(reverted, skipped, failed, warnings);
    }
}
