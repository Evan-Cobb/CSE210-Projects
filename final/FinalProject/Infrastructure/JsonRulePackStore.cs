using System.Text.Json;
using System.Text.Json.Serialization;
using FinalProject.Domain;

namespace FinalProject.Infrastructure;

public sealed class JsonRulePackStore
{
    private readonly IFileSystem _fileSystem;
    private readonly JsonSerializerOptions _options;

    public JsonRulePackStore(IFileSystem fileSystem)
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

    public RulePack Load(string path)
    {
        string json = _fileSystem.ReadAllText(path);
        return JsonSerializer.Deserialize<RulePack>(json, _options) ?? new RulePack();
    }

    public RulePack LoadOrCreateDefault(string path, Func<RulePack> defaultFactory, out bool created)
    {
        if (!_fileSystem.FileExists(path))
        {
            RulePack pack = defaultFactory();
            Save(path, pack);
            created = true;
            return pack;
        }

        created = false;
        return Load(path);
    }

    public void Save(string path, RulePack pack)
    {
        string json = JsonSerializer.Serialize(pack, _options);
        _fileSystem.WriteAllText(path, json);
    }
}
