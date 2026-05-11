using MedCore.Domain.Entities;

namespace MedCore.Application.Models;

public class MedCoreData
{
    public List<Patient> Patients { get; set; } = new();
    public List<Doctor> Doctors { get; set; } = new();
    public List<Nurse> Nurses { get; set; } = new();
    public List<Department> Departments { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}
