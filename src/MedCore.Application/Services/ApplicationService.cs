using MedCore.Application.Common;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;
/// <summary>
/// Handles appointment lifecycle operations.
/// </summary>
public class AppointmentService
{
    private readonly IAppointmentRepository _appointments;
    private readonly IRepository<Patient, int> _patients;
    private readonly IRepository<Doctor, int> _doctors;
    private readonly IValidationStrategy _strategy;
    private readonly IClock _clock;

    public AppointmentService(
        IAppointmentRepository appointments,
        IRepository<Patient, int> patients,
        IRepository<Doctor, int> doctors,
        IValidationStrategy strategy,
        IClock clock)
    {
        _appointments = appointments;
        _patients = patients;
        _doctors = doctors;
        _strategy = strategy;
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <summary>Creates a new appointment if business rules are satisfied.</summary>
    public Result<Appointment> CreateAppointment(int patientId, int doctorId, DateTime time)
    {
        if (time <= _clock.Now)
            return Result<Appointment>.Failure("Час прийому має бути у майбутньому.");

        if (_patients.GetById(patientId) is null)
            return Result<Appointment>.Failure("Пацієнта не знайдено.");

        if (_doctors.GetById(doctorId) is null)
            return Result<Appointment>.Failure("Лікаря не знайдено.");

        var id = IdGenerator.NextId(_appointments);
        Appointment appointment;
        try
        {
            appointment = new Appointment(id, patientId, doctorId, time);
        }
        catch (ArgumentException ex)
        {
            return Result<Appointment>.Failure(ex.Message);
        }

        var existing = _appointments.GetAll();
        if (!_strategy.IsValid(appointment, existing))
            return Result<Appointment>.Failure("Час прийому конфліктує з існуючими записами.");

        _appointments.Add(appointment);
        return Result<Appointment>.Success(appointment, "Запис створено.");
    }

    /// <summary>Confirms a pending appointment.</summary>
    public Result ConfirmAppointment(int appointmentId)
    {
        var appointment = _appointments.GetById(appointmentId);
        if (appointment is null)
            return Result.Failure("Запис не знайдено.");

        try
        {
            appointment.Confirm();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        _appointments.Update(appointment);
        return Result.Success("Запис підтверджено.");
    }

    /// <summary>Cancels an appointment.</summary>
    public Result CancelAppointment(int appointmentId)
    {
        var appointment = _appointments.GetById(appointmentId);
        if (appointment is null)
            return Result.Failure("Запис не знайдено.");

        try
        {
            appointment.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        _appointments.Update(appointment);
        return Result.Success("Запис скасовано.");
    }
}