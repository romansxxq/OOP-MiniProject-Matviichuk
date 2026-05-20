using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Strategies;
using Xunit;

namespace MedCore.Tests;

public class AppointmentRulesTests
{
    private static FixedClock CreateClock()
    {
        return new FixedClock(new DateTime(2026, 5, 20, 10, 0, 0));
    }

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
        var clock = CreateClock();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, new TimeSlotValidationStrategy(), clock);

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var result = service.CreateAppointment(1, 1, clock.Now.AddDays(-1));

        Assert.False(result.IsSuccess);
        Assert.Empty(appointmentRepo.GetAll());
    }

    [Fact]
    public void CreateAppointment_ShouldRejectUnknownPatient()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var clock = CreateClock();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, new TimeSlotValidationStrategy(), clock);

        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var result = service.CreateAppointment(99, 1, clock.Now.AddDays(1));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void CreateAppointment_ShouldRejectUnknownDoctor()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var clock = CreateClock();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, new TimeSlotValidationStrategy(), clock);

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));

        var result = service.CreateAppointment(1, 99, clock.Now.AddDays(1));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ConfirmAppointment_ShouldFail_WhenAlreadyConfirmed()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var clock = CreateClock();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, new TimeSlotValidationStrategy(), clock);

        var appointment = new Appointment(1, 1, 1, clock.Now.AddDays(1));
        appointmentRepo.Add(appointment);

        var first = service.ConfirmAppointment(1);
        var second = service.ConfirmAppointment(1);

        Assert.True(first.IsSuccess);
        Assert.False(second.IsSuccess);
    }
}
