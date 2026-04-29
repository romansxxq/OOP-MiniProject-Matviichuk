using MedCore.Domain.ValueObjects;
namespace MedCore.Domain.Entities;
public class Patient
{
    public int Id {get; private set;}
    public FullName Name {get; private set;}
    public string MedicalCardNumber {get; private set;}

    public Patient(int id, FullName name, string medicalCardNumber)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(medicalCardNumber)) throw new ArgumentException("Medical card number cannot be empty.", nameof(medicalCardNumber));
        Id = id;
        Name = name;
        MedicalCardNumber = medicalCardNumber;
    }
}