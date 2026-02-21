using FinalProject.Core;
using FinalProject.World;

namespace FinalProject.Game;

public static class ScenarioGenerator
{
    private static readonly string[] BaseNames = new[]
    {
        "tax-return-2024.pdf",
        "car-insurance-policy.pdf",
        "lease-agreement.pdf",
        "flight-itinerary.pdf",
        "boarding-pass.pdf",
        "bank-statement-january.pdf",
        "credit-card-statement.pdf",
        "electricity-bill.pdf",
        "water-bill.pdf",
        "internet-bill.pdf",
        "paystub-april.pdf",
        "school-transcript.pdf",
        "application-form.pdf",
        "passport-scan.pdf",
        "medical-bill-march.pdf",
        "doctor-visit-summary.pdf",
        "warranty-card.pdf",
        "mortgage-summary.pdf",
        "event-ticket.pdf",
        "invoice-1042.pdf",
        "resume.pdf",

        "grocery-list.docx",
        "weekly-meal-plan.docx",
        "cover-letter.docx",
        "meeting-agenda.docx",
        "meeting-minutes.docx",
        "trip-packing-list.docx",
        "project-plan.docx",

        "monthly-budget.xlsx",
        "expense-tracker.xlsx",
        "work-hours.xlsx",
        "annual-budget.xlsx",
        "budget-2025.xlsx",
        "mileage-log.xlsx",
        "class-slides.pptx",
        "project-update.pptx",
        "training-deck.pptx",

        "notes.txt",
        "journal-entry.txt",
        "wifi-passwords.txt",
        "todo-list.txt",
        "bookmarks.txt",
        "summary-report.md",
        "daily-notes.md",
        "release-notes.md",
        "contacts.csv",
        "contacts-backup.csv",
        "subscription-list.csv",

        "archive.zip",
        "phone-backup.zip",
        "old-projects.zip",
        "photos-2023.zip",
        "tax-docs-archive.zip",
        "chat-export.zip",
        "backup-photos.7z",
        "source-bundle.tar",
        "server-logs.tar.gz",
        "release-package.rar",

        "setup.deb",
        "security-update.deb",
        "update-installer.msi",
        "vpn-client.msi",
        "app-setup.exe",
        "printer-driver.exe",
        "video-editor-setup.exe",
        "hotfix.pkg",
        "office-installer.pkg",

        "song.mp3",
        "favorite-song.mp3",
        "workout-playlist.mp3",
        "commute-mix.mp3",
        "audiobook-chapter1.mp3",
        "language-lesson-05.mp3",
        "sleep-sounds.mp3",
        "karaoke-track.mp3",
        "birthday-voice-message.mp3",
        "podcast-episode.wav",
        "ambient-loop.flac",
        "voice-note.m4a",
        "theme-song.ogg",

        "video.mp4",
        "birthday-party.mp4",
        "vacation-highlights.mp4",
        "screen-recording.mp4",
        "soccer-game.mp4",
        "pet-video.mp4",
        "cooking-demo.mp4",
        "tutorial.mov",
        "capture.avi",
        "lecture.mkv",
        "clip.webm",

        "holiday.jpg",
        "family-photo.jpg",
        "beach-sunset.jpg",
        "receipt-photo.jpg",
        "menu-scan.jpg",
        "portrait.jpeg",
        "id-photo.jpeg",
        "pet-portrait.jpeg",
        "animated-banner.gif",
        "app-icon.svg",

        "code.cs",
        "unit-tests.cs",
        "invoice-parser.cs",
        "script.py",
        "weather-app.py",
        "data-cleanup.py",
        "frontend.js",
        "budget-calculator.js",
        "chat-widget.js",
        "backend.ts",
        "auth-service.ts",
        "main.java",
        "index.html",
        "styles.css",
        "config.json",
        "appsettings.json",
        "query.sql",
        "schema-migration.sql",
        "build.ps1",
        "deploy-script.ps1"
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

        int moveLimit = count + difficulty switch
        {
            Difficulty.Easy => 6,
            Difficulty.Normal => 4,
            _ => 2
        };

        return new Scenario(seed, difficulty, moveLimit, vfs, truth, items.AsReadOnly());
    }

    private static string GenerateFileName(Random rng, HashSet<string> usedNames)
    {
        string name;
        int roll = rng.Next(0, BaseNames.Length + 5);
        if (roll == 0)
        {
            name = $"IMG_{rng.Next(1000, 10000)}.png";
        }
        else if (roll == 1)
        {
            name = $"Screenshot_{rng.Next(100, 999)}.jpg";
        }
        else if (roll == 2)
        {
            name = $"Notes_{rng.Next(1, 53)}.md";
        }
        else if (roll == 3)
        {
            name = $"Recording_{rng.Next(1, 365)}.wav";
        }
        else if (roll == 4)
        {
            name = $"Archive_{rng.Next(2019, 2027)}.7z";
        }
        else
        {
            name = BaseNames[roll - 5];
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
            ".jpg" => "Pictures",
            ".jpeg" => "Pictures",
            ".gif" => "Pictures",
            ".svg" => "Pictures",

            ".pdf" => "Documents",
            ".txt" => "Documents",
            ".docx" => "Documents",
            ".xlsx" => "Documents",
            ".pptx" => "Documents",
            ".md" => "Documents",
            ".csv" => "Documents",

            ".zip" => "Archives",
            ".7z" => "Archives",
            ".rar" => "Archives",
            ".tar" => "Archives",
            ".gz" => "Archives",
            ".tgz" => "Archives",

            ".deb" => "Installers",
            ".msi" => "Installers",
            ".exe" => "Installers",
            ".pkg" => "Installers",

            ".mp3" => "Audio",
            ".wav" => "Audio",
            ".flac" => "Audio",
            ".m4a" => "Audio",
            ".ogg" => "Audio",

            ".mp4" => "Video",
            ".mov" => "Video",
            ".avi" => "Video",
            ".mkv" => "Video",
            ".webm" => "Video",

            ".cs" => "Code",
            ".py" => "Code",
            ".js" => "Code",
            ".ts" => "Code",
            ".java" => "Code",
            ".html" => "Code",
            ".css" => "Code",
            ".json" => "Code",
            ".sql" => "Code",
            ".ps1" => "Code",

            _ => "Misc"
        };
    }
}
