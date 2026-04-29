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
        var appointment = new Appointment(1, 101, 201, DateTime.Now);
        appointment.Cancel(AppointmentStatus.Cancelled);
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
        var repo = new InMemoryAppointmentRepository();
        var strategy = new TimeSlotValidationStrategy();
        var service = new AppointmentService(repo, strategy);

        var result = service.CreateAppointment(101, 201, DateTime.Now.AddDays(1));

        Assert.True(result.IsSuccess);
        Assert.Equal("Result: Appointment created successfully!", result.Message);
        Assert.Single(repo.GetAll()); 
    }

    [Fact]
    public void CreateAppointment_ShouldReturnFailure_WhenTimeIsTaken()
    {
        var repo = new InMemoryAppointmentRepository();
        var strategy = new TimeSlotValidationStrategy();
        var service = new AppointmentService(repo, strategy);

        var doctorId = 201;
        var time = DateTime.Now.AddDays(1);

        service.CreateAppointment(101, doctorId, time);
        var result = service.CreateAppointment(102, doctorId, time);

        Assert.False(result.IsSuccess);
        Assert.Equal("Error: Appointment time conflicts with existing appointments for the doctor.", result.Message);
        Assert.Single(repo.GetAll()); 
    }
}