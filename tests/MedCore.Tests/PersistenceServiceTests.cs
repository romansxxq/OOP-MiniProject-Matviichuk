using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Stores;
using Xunit;

namespace MedCore.Tests;

[Trait("Category", "Integration")]
public class PersistenceServiceTests
{
    [Fact]
    public async Task SaveAndLoad_ShouldRoundTrip()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            var store = new JsonFileDataStore(filePath);
            var patients = new InMemoryRepository<Patient, int>();
            var doctors = new InMemoryRepository<Doctor, int>();
            var nurses = new InMemoryRepository<Nurse, int>();
            var departments = new InMemoryRepository<Department, int>();
            var appointments = new InMemoryAppointmentRepository();

            patients.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
            doctors.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));
            departments.Add(new Department(1, "Surgery", 1));
            appointments.Add(new Appointment(1, 1, 1, DateTime.Now.AddDays(1)));

            var persistence = new PersistenceService(store, patients, doctors, nurses, departments, appointments);
            var saveResult = await persistence.SaveAsync();

            Assert.True(saveResult.IsSuccess);

            var patients2 = new InMemoryRepository<Patient, int>();
            var doctors2 = new InMemoryRepository<Doctor, int>();
            var nurses2 = new InMemoryRepository<Nurse, int>();
            var departments2 = new InMemoryRepository<Department, int>();
            var appointments2 = new InMemoryAppointmentRepository();

            var persistence2 = new PersistenceService(store, patients2, doctors2, nurses2, departments2, appointments2);
            var loadResult = await persistence2.LoadAsync();

            Assert.True(loadResult.IsSuccess);
            Assert.Single(patients2.GetAll());
            Assert.Single(doctors2.GetAll());
            Assert.Single(departments2.GetAll());
            Assert.Single(appointments2.GetAll());
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task Load_ShouldFail_OnCorruptedJson()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            await File.WriteAllTextAsync(filePath, "{ invalid json");

            var store = new JsonFileDataStore(filePath);
            var patients = new InMemoryRepository<Patient, int>();
            var doctors = new InMemoryRepository<Doctor, int>();
            var nurses = new InMemoryRepository<Nurse, int>();
            var departments = new InMemoryRepository<Department, int>();
            var appointments = new InMemoryAppointmentRepository();

            var persistence = new PersistenceService(store, patients, doctors, nurses, departments, appointments);
            var loadResult = await persistence.LoadAsync();

            Assert.False(loadResult.IsSuccess);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
