using MedCore.Application.Common;
using MedCore.Application.Factories;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;

public class DoctorService
{
    private readonly IRepository<Doctor, int> _doctors;
    private readonly StaffFactory _factory;

    public DoctorService(IRepository<Doctor, int> doctors, StaffFactory factory)
    {
        _doctors = doctors;
        _factory = factory;
    }

    public Result<Doctor> RegisterDoctor(string fullName, string specialization)
    {
        try
        {
            var id = IdGenerator.NextId(_doctors);
            var staff = _factory.CreateStaff(id, fullName, specialization);
            if (staff is not Doctor doctor)
                return Result<Doctor>.Failure("Не вдалося створити лікаря.");

            _doctors.Add(doctor);
            return Result<Doctor>.Success(doctor, "Лікаря додано.");
        }
        catch (ArgumentException ex)
        {
            return Result<Doctor>.Failure(ex.Message);
        }
    }
}
