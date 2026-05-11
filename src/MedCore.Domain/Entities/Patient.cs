using System.Text.Json.Serialization;
using MedCore.Domain.Abstractions;
using MedCore.Domain.ValueObjects;
namespace MedCore.Domain.Entities;
public class Patient : IEntity<int>
{
    public int Id {get; private set;}
    public FullName Name {get; private set;}
    public string MedicalCardNumber {get; private set;}

    [JsonConstructor]
    public Patient(int id, FullName name, string medicalCardNumber)
    {
        if (id <= 0) throw new ArgumentException("Patient id must be positive.", nameof(id));
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(name.FirstName)) throw new ArgumentException("First name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(name.LastName)) throw new ArgumentException("Last name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(medicalCardNumber)) throw new ArgumentException("Medical card number cannot be empty.", nameof(medicalCardNumber));
        Id = id;
        Name = name;
        MedicalCardNumber = medicalCardNumber.Trim();
    }
}