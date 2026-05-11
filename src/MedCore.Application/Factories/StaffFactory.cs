using MedCore.Domain.Entities;
using MedCore.Domain.ValueObjects;
using MedCore.Domain.Abstractions;
namespace MedCore.Application.Factories;
public abstract class StaffFactory
{
    public abstract MedicalStaff CreateStaff(int id, string name, string spec);
}
public class DoctorFactory : StaffFactory
{
    public override MedicalStaff CreateStaff(int id, string name, string spec)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
            throw new ArgumentException("Full name must include first and last name.", nameof(name));

        var fullName = new FullName(parts[0], parts[^1]);
        return new Doctor(id, fullName, spec);
    }
}