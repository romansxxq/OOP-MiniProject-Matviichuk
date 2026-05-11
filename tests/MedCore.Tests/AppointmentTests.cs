using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Strategies;
using Xunit;

namespace MedCore.Tests;

public class AppointmentTests
{
    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var appointment = new Appointment(1, 101, 201, DateTime.Now.AddHours(2));
        appointment.Cancel();
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }

    [Fact]
    public void Strategy_ShouldReturnFalse_WhenTimeIsAlreadyBooked()
    {
        var strategy = new TimeSlotValidationStrategy();
        var doctorId = 201;
        var time = new DateTime(2026, 1, 1, 10, 0, 0);

        var existingAppointment = new Appointment(1, 101, doctorId, time);
        var existingList = new List<Appointment> { existingAppointment };
        var newAppointment = new Appointment(2, 102, doctorId, time);

        var isValid = strategy.IsValid(newAppointment, existingList);

        Assert.False(isValid);
    }

    [Fact]
    public void CreateAppointment_ShouldReturnSuccess_WhenTimeIsFree()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var strategy = new TimeSlotValidationStrategy();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, strategy);

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var result = service.CreateAppointment(1, 1, DateTime.Now.AddDays(1));

        Assert.True(result.IsSuccess);
        Assert.Single(appointmentRepo.GetAll());
    }

    [Fact]
    public void CreateAppointment_ShouldReturnFailure_WhenTimeIsTaken()
    {
        var appointmentRepo = new InMemoryAppointmentRepository();
        var patientRepo = new InMemoryRepository<Patient, int>();
        var doctorRepo = new InMemoryRepository<Doctor, int>();
        var strategy = new TimeSlotValidationStrategy();
        var service = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, strategy);

        patientRepo.Add(new Patient(1, new MedCore.Domain.ValueObjects.FullName("Olena", "Kovalenko"), "MC-01"));
        patientRepo.Add(new Patient(2, new MedCore.Domain.ValueObjects.FullName("Maria", "Koval"), "MC-02"));
        doctorRepo.Add(new Doctor(1, new MedCore.Domain.ValueObjects.FullName("Ivan", "Petrenko"), "Hirurg"));

        var time = DateTime.Now.AddDays(1);
        service.CreateAppointment(1, 1, time);
        var result = service.CreateAppointment(2, 1, time);

        Assert.False(result.IsSuccess);
        Assert.Single(appointmentRepo.GetAll());
    }
}