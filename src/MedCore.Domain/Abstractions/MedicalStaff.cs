using MedCore.Domain.ValueObjects;
namespace MedCore.Domain.Abstractions;
public abstract class MedicalStaff : IEntity<int>
{
    public int Id { get; private set; }
    public FullName Name { get; private set; }
    public string Specialization { get; protected set; }

    protected MedicalStaff(int id, FullName name, string specialization)
    {
        Id = id;
        Name = name;
        Specialization = specialization;
    }
    public abstract void PerformDuty();
}