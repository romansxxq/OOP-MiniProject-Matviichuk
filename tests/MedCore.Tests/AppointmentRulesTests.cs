using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Strategies;
using Xunit;

namespace MedCore.Tests;

public class AppointmentRulesTests
{
    [Fact]
    public void Confirm_ShouldRequireNewStatus()
    {
        var appointment = new Appointment(1, 1, 1, DateTime.Now.AddDays(1));
        appointment.Confirm();

        Assert.Throws<InvalidOperationException>(() => appointment.Confirm());
        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void CreateAppointment_ShouldRejectPastTime()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, new TimeSlotValidationStrategy());

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var result = service.CreateAppointment(1, 1, DateTime.Now.AddDays(-1));

        Assert.False(result.IsSuccess);
        Assert.Empty(appointmentRepo.GetAll());
    }

    [Fact]
    public void CreateAppointment_ShouldRejectUnknownPatient()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, new TimeSlotValidationStrategy());

        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var result = service.CreateAppointment(99, 1, DateTime.Now.AddDays(1));

        Assert.False(result.IsSuccess);
    }
}
