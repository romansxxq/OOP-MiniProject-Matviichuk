using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Infrastructure.Repositories;
using Xunit;

namespace MedCore.Tests;

public class QueryServiceTests
{
    [Fact]
    public void GetActiveAppointments_ShouldExcludeCancelled_AndSort()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var now = DateTime.Now;
        var first = new Appointment(1, 1, 1, now.AddDays(2));
        var second = new Appointment(2, 1, 1, now.AddDays(1));
        var cancelled = new Appointment(3, 1, 1, now.AddDays(3));
        cancelled.Cancel();

        appointmentRepo.Add(first);
        appointmentRepo.Add(second);
        appointmentRepo.Add(cancelled);

        var service = new QueryService(appointmentRepo, patientRepo, doctorRepo);
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

        var service = new QueryService(appointmentRepo, patientRepo, doctorRepo);
        var result = service.SearchPatients("lena", "MC").ToList();

        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }
}
