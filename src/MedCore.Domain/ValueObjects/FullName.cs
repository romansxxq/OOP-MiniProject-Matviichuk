using System.Text.Json.Serialization;

namespace MedCore.Domain.ValueObjects;
public sealed record FullName
{
    public string FirstName { get; init; }
    public string LastName { get; init; }

    [JsonConstructor]
    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public override string ToString() => $"{FirstName} {LastName}";
}