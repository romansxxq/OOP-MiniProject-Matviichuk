using MedCore.Domain.Entities;
using MedCore.Domain.ValueObjects;
using MedCore.Domain.Abstractions;
namespace MedCore.Application.Factories;
public abstract class StaffFactory
{
    public abstract MedicalStaff CreateStaff(string name, string spec);
}
public class DoctorFactory : StaffFactory
{
    public override MedicalStaff CreateStaff(string name, string spec)
    {
        var parts = name.Split(' ');
        var fullName = new FullName(parts[0], parts.Length > 1 ? parts[1] : "");
        var randomId = new Random().Next(1, 1000); // Simulate ID generation
        return new Doctor(randomId, fullName, spec);
    }
}