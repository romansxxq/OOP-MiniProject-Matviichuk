using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Infrastructure.Repositories;
using Xunit;

namespace MedCore.Tests;

public class QueryServiceTests
{
    private static FixedClock CreateClock(DateTime now)
    {
        return new FixedClock(now);
    }

    [Fact]
    public void GetActiveAppointments_ShouldExcludeCancelled_AndSort()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var now = new DateTime(2026, 5, 20, 10, 0, 0);
        var first = new Appointment(1, 1, 1, now.AddDays(2));
        var second = new Appointment(2, 1, 1, now.AddDays(1));
        var cancelled = new Appointment(3, 1, 1, now.AddDays(3));
        cancelled.Cancel();

        appointmentRepo.Add(first);
        appointmentRepo.Add(second);
        appointmentRepo.Add(cancelled);

        var service = new QueryService(appointmentRepo, patientRepo, doctorRepo, CreateClock(now));
        var active = service.GetActiveAppointments().ToList();

        Assert.Equal(2, active.Count);
        Assert.DoesNotContain(active, a => a.Status == AppointmentStatus.Cancelled);
        Assert.Equal(second.Id, active[0].Id);
        Assert.Equal(first.Id, active[1].Id);
    }

    [Fact]
    public void SearchPatients_ShouldUseNameAndCard()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Iryna", "Melnyk"), "CARD-01"));
        patientRepo.Add(new Patient(2, new MedCore.Domain.ValueObjects.FullName("Olena", "Koval"), "MC-02"));

        var service = new QueryService(appointmentRepo, patientRepo, doctorRepo, CreateClock(new DateTime(2026, 5, 20, 10, 0, 0)));
        var result = service.SearchPatients("lena", "MC").ToList();

        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }

    [Fact]
    public void GetAppointmentStats_ShouldUseClockAndCountUpcoming()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var now = new DateTime(2026, 5, 20, 10, 0, 0);
        var upcomingNew = new Appointment(1, 1, 1, now.AddDays(1));
        var upcomingConfirmed = new Appointment(2, 1, 1, now.AddDays(2));
        upcomingConfirmed.Confirm();
        var cancelled = new Appointment(3, 1, 1, now.AddDays(3));
        cancelled.Cancel();
        var past = new Appointment(4, 1, 1, now.AddDays(-1));

        appointmentRepo.Add(upcomingNew);
        appointmentRepo.Add(upcomingConfirmed);
        appointmentRepo.Add(cancelled);
        appointmentRepo.Add(past);

        var service = new QueryService(appointmentRepo, patientRepo, doctorRepo, CreateClock(now));
        var stats = service.GetAppointmentStats();

        Assert.Equal(4, stats.Total);
        Assert.Equal(2, stats.NewCount);
        Assert.Equal(1, stats.ConfirmedCount);
        Assert.Equal(1, stats.CancelledCount);
        Assert.Equal(2, stats.UpcomingCount);
    }
}
