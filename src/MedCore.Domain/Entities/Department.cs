using System.Text.Json.Serialization;
using MedCore.Domain.Abstractions;

namespace MedCore.Domain.Entities;

public class Department : IEntity<int>
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Floor { get; private set; }

    [JsonConstructor]
    public Department(int id, string name, int floor)
    {
        if (id <= 0) throw new ArgumentException("Department id must be positive.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Department name cannot be empty.", nameof(name));
        if (floor < 0) throw new ArgumentException("Floor cannot be negative.", nameof(floor));

        Id = id;
        Name = name.Trim();
        Floor = floor;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Department name cannot be empty.", nameof(name));
        Name = name.Trim();
    }
}
