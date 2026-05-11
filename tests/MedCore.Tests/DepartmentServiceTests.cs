using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Infrastructure.Repositories;
using Xunit;

namespace MedCore.Tests;

public class DepartmentServiceTests
{
    [Fact]
    public void CreateDepartment_ShouldCreateDepartment()
    {
        var departmentRepo = new InMemoryRepository<Department, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var service = new DepartmentService(departmentRepo, doctorRepo);

        var result = service.CreateDepartment("Cardiology", 2);

        Assert.True(result.IsSuccess);
        Assert.Single(departmentRepo.GetAll());
    }

    [Fact]
    public void AssignDoctorToDepartment_ShouldSetDepartmentId()
    {
        var departmentRepo = new InMemoryRepository<Department, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var service = new DepartmentService(departmentRepo, doctorRepo);

        var department = new Department(1, "Surgery", 1);
        departmentRepo.Add(department);
        var doctor = new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg");
        doctorRepo.Add(doctor);

        var result = service.AssignDoctorToDepartment(1, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, doctorRepo.GetById(1)!.DepartmentId);
    }

    [Fact]
    public void AssignDoctorToDepartment_ShouldFail_WhenAlreadyAssigned()
    {
        var departmentRepo = new InMemoryRepository<Department, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var service = new DepartmentService(departmentRepo, doctorRepo);

        departmentRepo.Add(new Department(1, "Surgery", 1));
        departmentRepo.Add(new Department(2, "Therapy", 2));
        var doctor = new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg");
        doctor.AssignToDepartment(1);
        doctorRepo.Add(doctor);

        var result = service.AssignDoctorToDepartment(1, 2);

        Assert.False(result.IsSuccess);
        Assert.Equal(1, doctorRepo.GetById(1)!.DepartmentId);
    }
}
