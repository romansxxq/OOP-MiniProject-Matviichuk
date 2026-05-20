using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Infrastructure.Repositories;
using Xunit;

namespace MedCore.Tests;

public class PersistenceFaultHandlingTests
{
    [Fact]
    public async Task Load_ShouldReturnFailure_AndReport_OnIOException()
    {
        var reporter = new RecordingErrorReporter();
        var store = new ThrowingDataStore(loadException: new IOException("Read failed."));
        var persistence = CreatePersistence(store, reporter);

        var result = await persistence.LoadAsync();

        Assert.False(result.IsSuccess);
        Assert.Single(reporter.Entries);
    }

    [Fact]
    public async Task Save_ShouldReturnFailure_AndReport_OnIOException()
    {
        var reporter = new RecordingErrorReporter();
        var store = new ThrowingDataStore(saveException: new IOException("Write failed."));
        var persistence = CreatePersistence(store, reporter);

        var result = await persistence.SaveAsync();

        Assert.False(result.IsSuccess);
        Assert.Single(reporter.Entries);
    }

    private static PersistenceService CreatePersistence(ThrowingDataStore store, RecordingErrorReporter reporter)
    {
        var patients = new InMemoryRepository<Patient, int>();
        var doctors = new InMemoryRepository<Doctor, int>();
        var nurses = new InMemoryRepository<Nurse, int>();
        var departments = new InMemoryRepository<Department, int>();
        var appointments = new InMemoryAppointmentRepository();

        return new PersistenceService(store, patients, doctors, nurses, departments, appointments, reporter);
    }
}
