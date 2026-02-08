using System.Text.Json;
using System.Text.Json.Serialization;
using FinalProject.Domain;

namespace FinalProject.Infrastructure;

public sealed class JsonRunLogStore
{
    private readonly IFileSystem _fileSystem;
    private readonly JsonSerializerOptions _options;

    public JsonRunLogStore(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public RunLog Load(string path)
    {
        string json = _fileSystem.ReadAllText(path);
        return JsonSerializer.Deserialize<RunLog>(json, _options) ?? new RunLog();
    }

    public void Save(string path, RunLog log)
    {
        string json = JsonSerializer.Serialize(log, _options);
        _fileSystem.WriteAllText(path, json);
    }
}
