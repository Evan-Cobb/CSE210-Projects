using FinalProject.Core;
using FinalProject.World;

namespace FinalProject.Game;

public static class ScenarioGenerator
{
    private static readonly string[] BaseNames = new[]
    {
        "resume.pdf",
        "notes.txt",
        "archive.zip",
        "setup.deb",
        "video.mp4",
        "song.mp3",
        "code.cs"
    };

    private static readonly string[] FolderNames = new[]
    {
        "Inbox",
        "Pictures",
        "Documents",
        "Archives",
        "Installers",
        "Audio",
        "Video",
        "Code",
        "Misc"
    };

    public static IReadOnlyList<string> DefaultFolderNames => Array.AsReadOnly(FolderNames);

    public static Scenario Generate(int seed, Difficulty difficulty)
    {
        Random rng = new Random(seed);

        int count = difficulty switch
        {
            Difficulty.Easy => rng.Next(8, 13),
            Difficulty.Normal => rng.Next(12, 19),
            _ => rng.Next(18, 26)
        };

        VirtualFileSystem vfs = new VirtualFileSystem(FolderNames);
        List<VirtualFileItem> items = new List<VirtualFileItem>(count);
        Dictionary<Guid, string> truth = new Dictionary<Guid, string>();

        HashSet<string> usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        DateTime baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime startDate = baseDate.AddYears(-3);
        int rangeDays = (int)(baseDate - startDate).TotalDays;

        for (int i = 0; i < count; i++)
        {
            string name = GenerateFileName(rng, usedNames);
            DateTime created = startDate.AddDays(rng.Next(0, rangeDays + 1))
                                        .AddHours(rng.Next(0, 24))
                                        .AddMinutes(rng.Next(0, 60));
            DateTime modified = created.AddDays(rng.Next(0, 365));
            if (modified > baseDate)
            {
                modified = baseDate;
            }

            VirtualFileItem item = new VirtualFileItem(name, created, modified);
            items.Add(item);
            vfs.AddToFolder("Inbox", item);

            truth[item.Id] = MapToFolder(item);
        }

        int turnLimit = count + difficulty switch
        {
            Difficulty.Easy => 6,
            Difficulty.Normal => 4,
            _ => 2
        };

        return new Scenario(seed, difficulty, turnLimit, vfs, truth, items.AsReadOnly());
    }

    private static string GenerateFileName(Random rng, HashSet<string> usedNames)
    {
        string name;
        int roll = rng.Next(0, BaseNames.Length + 1);
        if (roll == 0)
        {
            name = $"IMG_{rng.Next(1000, 10000)}.png";
        }
        else
        {
            name = BaseNames[roll - 1];
        }

        return EnsureUnique(name, usedNames);
    }

    private static string EnsureUnique(string name, HashSet<string> usedNames)
    {
        if (usedNames.Add(name))
        {
            return name;
        }

        string baseName = Path.GetFileNameWithoutExtension(name);
        string extension = Path.GetExtension(name);
        int suffix = 2;

        while (true)
        {
            string candidate = $"{baseName}_{suffix}{extension}";
            if (usedNames.Add(candidate))
            {
                return candidate;
            }
            suffix++;
        }
    }

    private static string MapToFolder(VirtualFileItem item)
    {
        string ext = item.Extension.ToLowerInvariant();
        return ext switch
        {
            ".png" => "Pictures",
            ".pdf" => "Documents",
            ".txt" => "Documents",
            ".zip" => "Archives",
            ".deb" => "Installers",
            ".mp3" => "Audio",
            ".mp4" => "Video",
            ".cs" => "Code",
            _ => "Misc"
        };
    }
}
