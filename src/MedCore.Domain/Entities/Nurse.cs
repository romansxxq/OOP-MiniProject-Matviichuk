using MedCore.Domain.ValueObjects;
using MedCore.Domain.Abstractions;
namespace MedCore.Domain.Entities;

public class Nurse : MedicalStaff
{
    public int FloorLevel { get; private set; }
    public Nurse(int id, FullName name, string specialization, int floorLevel) : base(id, name, specialization)
    {
        if (floorLevel < 0)
            throw new ArgumentException("Floor level cannot be negative.", nameof(floorLevel));
        FloorLevel = floorLevel;
    }

    public override void PerformDuty()
    {
        Console.WriteLine($"{Name} is performing nursing duties on floor {FloorLevel}.");
    }
    public void ReassignFloor(int newFloorLevel)
    {
        if (newFloorLevel < 0) throw new ArgumentException("Floor level cannot be negative.", nameof(newFloorLevel));
        FloorLevel = newFloorLevel;
        Console.WriteLine($"{Name} has been reassigned to floor {FloorLevel}.");
    }
}