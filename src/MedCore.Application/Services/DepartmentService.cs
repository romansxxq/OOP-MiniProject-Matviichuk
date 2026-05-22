using MedCore.Application.Common;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;

/// <summary>
/// Handles department management and assignments.
/// </summary>
public class DepartmentService
{
    private readonly IRepository<Department, int> _departments;
    private readonly IRepository<Doctor, int> _doctors;

    public DepartmentService(IRepository<Department, int> departments, IRepository<Doctor, int> doctors)
    {
        _departments = departments;
        _doctors = doctors;
    }

    /// <summary>Creates a new department.</summary>
    public Result<Department> CreateDepartment(string name, int floor)
    {
        try
        {
            var id = IdGenerator.NextId(_departments);
            var department = new Department(id, name, floor);
            _departments.Add(department);
            return Result<Department>.Success(department, "Відділення створено.");
        }
        catch (ArgumentException ex)
        {
            return Result<Department>.Failure(ex.Message);
        }
    }

    /// <summary>Assigns a doctor to a department.</summary>
    public Result AssignDoctorToDepartment(int doctorId, int departmentId)
    {
        var doctor = _doctors.GetById(doctorId);
        if (doctor is null)
            return Result.Failure("Лікаря не знайдено.");

        var department = _departments.GetById(departmentId);
        if (department is null)
            return Result.Failure("Відділення не знайдено.");

        try
        {
            doctor.AssignToDepartment(departmentId);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        _doctors.Update(doctor);
        return Result.Success("Лікаря призначено до відділення.");
    }
}
