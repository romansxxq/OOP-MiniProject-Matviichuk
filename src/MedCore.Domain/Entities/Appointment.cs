using System.Text.Json.Serialization;
using MedCore.Domain.Abstractions;
using MedCore.Domain.Enums;
namespace MedCore.Domain.Entities;
public class Appointment : IEntity<int>
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public int DoctorId { get; private set; }
    public DateTime AppointmentTime { get; private set; }
    public AppointmentStatus Status { get; private set; }

    [JsonConstructor]
    public Appointment(int id, int patientId, int doctorId, DateTime appointmentTime, AppointmentStatus status)
    {
        if (id <= 0) throw new ArgumentException("Appointment id must be positive.", nameof(id));
        if (patientId <= 0) throw new ArgumentException("Patient id must be positive.", nameof(patientId));
        if (doctorId <= 0) throw new ArgumentException("Doctor id must be positive.", nameof(doctorId));
        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentTime = appointmentTime;
        Status = status;
    }

    public Appointment(int id, int patientId, int doctorId, DateTime appointmentTime)
        : this(id, patientId, doctorId, appointmentTime, AppointmentStatus.New)
    {
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.New)
            throw new InvalidOperationException("Only new appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Appointment is already cancelled.");

        Status = AppointmentStatus.Cancelled;
    }
}