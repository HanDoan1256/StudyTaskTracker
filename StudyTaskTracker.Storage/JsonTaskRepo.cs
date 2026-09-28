using System.Text.Json;
using System.Text.Json.Serialization;
using StudyTaskTracker.Core.Models;
using StudyTaskTracker.Core.Repositories;

namespace StudyTaskTracker.Storage;

public class JsonTaskRepo : ITaskRepo
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonTaskRepo(string filePath)
    {
        _filePath = filePath;

        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public List<TaskItem> LoadAll()
    {
        if (!File.Exists(_filePath))
        {
            return new List<TaskItem>();
        }

        string json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<List<TaskItem>>(
            json, _options) ?? new List<TaskItem>();
    }

    public void SaveAll(List<TaskItem> tasks)
    {
        string json = JsonSerializer.Serialize(tasks, _options);

        string temporaryPath = _filePath + ".tmp";

        File.WriteAllText(temporaryPath, json);

        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}