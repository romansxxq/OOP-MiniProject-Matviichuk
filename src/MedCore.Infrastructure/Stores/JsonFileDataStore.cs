using System.Text.Json;
using MedCore.Application.Models;
using MedCore.Domain.Interfaces;

namespace MedCore.Infrastructure.Stores;

public class JsonFileDataStore : IDataStore<MedCoreData>
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonFileDataStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<MedCoreData> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Data file not found.", _filePath);

        await using var stream = File.OpenRead(_filePath);
        var data = await JsonSerializer.DeserializeAsync<MedCoreData>(stream, _options, cancellationToken);
        return data ?? new MedCoreData();
    }

    public async Task SaveAsync(MedCoreData data, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, data, _options, cancellationToken);
    }
}
