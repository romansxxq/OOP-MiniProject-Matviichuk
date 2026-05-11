using System.Text.Json.Serialization;
using MedCore.Domain.ValueObjects;
using MedCore.Domain.Abstractions;
namespace MedCore.Domain.Entities;
public class Doctor : MedicalStaff
{
    private readonly List<string> _certificates = new();
    public IReadOnlyCollection<string> Certificates => _certificates.AsReadOnly();
    [JsonInclude]
    public int? DepartmentId { get; private set; }

    public Doctor(int id, FullName name, string specialization)
        : base(id, name, specialization)
    {
        if (string.IsNullOrWhiteSpace(specialization))
            throw new ArgumentException("Specialization must be specified.", nameof(specialization));
    }

    public override void PerformDuty()
    {
        Console.WriteLine($"{Name} is performing duty as a {Specialization}.");
    }

    public void AssignToDepartment(int departmentId)
    {
        if (departmentId <= 0) throw new ArgumentException("Department id must be positive.", nameof(departmentId));
        if (DepartmentId.HasValue && DepartmentId.Value != departmentId)
            throw new InvalidOperationException("Doctor is already assigned to another department.");

        DepartmentId = departmentId;
    }
    public void AddCertificate(string certificate)
    {
        if (string.IsNullOrWhiteSpace(certificate)) throw new ArgumentException("Certificate cannot be empty.", nameof(certificate));
        _certificates.Add(certificate);
    }
}