using MedCore.Application.Models;
using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Domain.Interfaces;
using MedCore.Domain.ValueObjects;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Stores;
using MedCore.Infrastructure.Strategies;
using Xunit;

namespace MedCore.Tests;

[Trait("Category", "Integration")]
public class PersistenceIntegrationTests
{
    [Fact]
    public async Task Load_ShouldReturnSuccess_WhenFileMissing()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);

        var repos = CreateRepositories();
        var persistence = CreatePersistence(filePath, repos);

        var result = await persistence.LoadAsync();

        Assert.True(result.IsSuccess);
        Assert.Empty(repos.Patients.GetAll());
        Assert.Empty(repos.Appointments.GetAll());
    }

    [Fact]
    public async Task SaveAndReload_ShouldPreserveCancelledStatus()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            var repos = CreateRepositories();
            repos.Patients.Add(new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01"));
            repos.Doctors.Add(new Doctor(1, new FullName("Ivan", "Petrenko"), "Hirurg"));

            var appointment = new Appointment(1, 1, 1, new DateTime(2026, 5, 21, 9, 0, 0));
            appointment.Cancel();
            repos.Appointments.Add(appointment);

            var persistence = CreatePersistence(filePath, repos);
            var saveResult = await persistence.SaveAsync();

            Assert.True(saveResult.IsSuccess);

            var repos2 = CreateRepositories();
            var persistence2 = CreatePersistence(filePath, repos2);
            var loadResult = await persistence2.LoadAsync();

            Assert.True(loadResult.IsSuccess);

            var reloaded = repos2.Appointments.GetById(1);
            Assert.NotNull(reloaded);
            Assert.Equal(AppointmentStatus.Cancelled, reloaded!.Status);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task SaveAndReload_ShouldAllowNewAppointmentAfterLoad()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            var now = new DateTime(2026, 5, 20, 10, 0, 0);
            var clock = new FixedClock(now);
            var validation = new CompositeValidationStrategy(new IValidationStrategy[]
            {
                new TimeSlotValidationStrategy(),
                new PatientTimeConflictValidationStrategy(),
                new DoctorDailyLimitValidationStrategy(10)
            });

            var repos = CreateRepositories();
            repos.Patients.Add(new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01"));
            repos.Doctors.Add(new Doctor(1, new FullName("Ivan", "Petrenko"), "Hirurg"));
            repos.Appointments.Add(new Appointment(1, 1, 1, now.AddDays(1)));

            var persistence = CreatePersistence(filePath, repos);
            var saveResult = await persistence.SaveAsync();

            Assert.True(saveResult.IsSuccess);

            var repos2 = CreateRepositories();
            var persistence2 = CreatePersistence(filePath, repos2);
            var loadResult = await persistence2.LoadAsync();

            Assert.True(loadResult.IsSuccess);
            Assert.Single(repos2.Appointments.GetAll());

            var appointmentService = new AppointmentService(
                repos2.Appointments,
                repos2.Patients,
                repos2.Doctors,
                validation,
                clock);

            var createResult = appointmentService.CreateAppointment(1, 1, now.AddDays(2));

            Assert.True(createResult.IsSuccess);
            Assert.Equal(2, repos2.Appointments.GetAll().Count);

            var secondSave = await persistence2.SaveAsync();
            Assert.True(secondSave.IsSuccess);

            var repos3 = CreateRepositories();
            var persistence3 = CreatePersistence(filePath, repos3);
            var reloadResult = await persistence3.LoadAsync();

            Assert.True(reloadResult.IsSuccess);
            Assert.Equal(2, repos3.Appointments.GetAll().Count);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task Load_ShouldFail_WhenDuplicatePatientIds()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            var store = new JsonFileDataStore(filePath);
            var data = new MedCoreData
            {
                Patients = new List<Patient>
                {
                    new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01"),
                    new Patient(1, new FullName("Iryna", "Melnyk"), "MC-02")
                }
            };

            await store.SaveAsync(data);

            var repos = CreateRepositories();
            var persistence = new PersistenceService(store, repos.Patients, repos.Doctors, repos.Nurses, repos.Departments, repos.Appointments);
            var result = await persistence.LoadAsync();

            Assert.False(result.IsSuccess);
            Assert.Empty(repos.Patients.GetAll());
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task Load_ShouldFail_WhenAppointmentHasUnknownDoctor()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            var store = new JsonFileDataStore(filePath);
            var data = new MedCoreData
            {
                Patients = new List<Patient>
                {
                    new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01")
                },
                Appointments = new List<Appointment>
                {
                    new Appointment(1, 1, 99, new DateTime(2026, 5, 21, 9, 0, 0))
                }
            };

            await store.SaveAsync(data);

            var repos = CreateRepositories();
            var persistence = new PersistenceService(store, repos.Patients, repos.Doctors, repos.Nurses, repos.Departments, repos.Appointments);
            var result = await persistence.LoadAsync();

            Assert.False(result.IsSuccess);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task Load_ShouldFail_WhenAppointmentHasUnknownPatient()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"medcore_{Guid.NewGuid()}.json");

        try
        {
            var store = new JsonFileDataStore(filePath);
            var data = new MedCoreData
            {
                Doctors = new List<Doctor>
                {
                    new Doctor(1, new FullName("Ivan", "Petrenko"), "Hirurg")
                },
                Appointments = new List<Appointment>
                {
                    new Appointment(1, 99, 1, new DateTime(2026, 5, 21, 9, 0, 0))
                }
            };

            await store.SaveAsync(data);

            var repos = CreateRepositories();
            var persistence = new PersistenceService(store, repos.Patients, repos.Doctors, repos.Nurses, repos.Departments, repos.Appointments);
            var result = await persistence.LoadAsync();

            Assert.False(result.IsSuccess);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    private static RepositorySet CreateRepositories()
    {
        return new RepositorySet(
            new InMemoryRepository<Patient, int>(),
            new InMemoryRepository<Doctor, int>(),
            new InMemoryRepository<Nurse, int>(),
            new InMemoryRepository<Department, int>(),
            new InMemoryAppointmentRepository());
    }

    private static PersistenceService CreatePersistence(string filePath, RepositorySet repos)
    {
        var store = new JsonFileDataStore(filePath);
        return new PersistenceService(store, repos.Patients, repos.Doctors, repos.Nurses, repos.Departments, repos.Appointments);
    }

    private sealed record RepositorySet(
        InMemoryRepository<Patient, int> Patients,
        InMemoryRepository<Doctor, int> Doctors,
        InMemoryRepository<Nurse, int> Nurses,
        InMemoryRepository<Department, int> Departments,
        InMemoryAppointmentRepository Appointments);
}
