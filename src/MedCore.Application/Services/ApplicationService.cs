using MedCore.Application.Common;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;
public class AppointmentService
{
    private readonly IAppointmentRepository _repository;
    private readonly IValidationStrategy _strategy;

    public AppointmentService(IAppointmentRepository repository, IValidationStrategy strategy)
    {
        _repository = repository;
        _strategy = strategy;
    }

    public Result CreateAppointment(int pId, int dId, DateTime time)
    {
        var tempId = new Random().Next(1, 10000); // Тимчасовий ID для запису
        var app = new Appointment(tempId, pId, dId, time);
        var existing = _repository.GetByDoctorId(dId);

        if (!_strategy.IsValid(app, existing))
        {
            return Result.Failure("Error: Appointment time conflicts with existing appointments for the doctor.");
        }

        _repository.Add(app);
        return Result.Success("Result: Appointment created successfully!");
    }
}