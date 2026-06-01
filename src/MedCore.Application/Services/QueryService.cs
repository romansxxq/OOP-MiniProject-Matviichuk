using MedCore.Application.Common;
using MedCore.Application.Models;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;

/// <summary>
/// Provides read-only queries and analytics.
/// </summary>
public class QueryService
{
    private readonly IAppointmentRepository _appointments;
    private readonly IRepository<Patient, int> _patients;
    private readonly IRepository<Doctor, int> _doctors;
    private readonly IClock _clock;

    public QueryService(
        IAppointmentRepository appointments,
        IRepository<Patient, int> patients,
        IRepository<Doctor, int> doctors,
        IClock clock)
    {
        _appointments = appointments;
        _patients = patients;
        _doctors = doctors;
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <summary>Returns active (non-cancelled) appointments ordered by time.</summary>
    public IReadOnlyCollection<AppointmentView> GetActiveAppointments()
    {
        return _appointments.GetAll()
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentTime)
            .Select(ToView)
            .ToList();
    }

    /// <summary>Returns appointments for a doctor ordered by time.</summary>
    public IReadOnlyCollection<AppointmentView> GetDoctorAppointments(int doctorId)
    {
        return _appointments.GetAll()
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.AppointmentTime)
            .Select(ToView)
            .ToList();
    }

    private AppointmentView ToView(Appointment a)
    {
        var patient = _patients.GetById(a.PatientId);
        var doctor = _doctors.GetById(a.DoctorId);
        return new AppointmentView
        {
            Id = a.Id,
            AppointmentTime = a.AppointmentTime,
            PatientName = patient is null ? $"#{a.PatientId}" : patient.Name.ToString(),
            DoctorName = doctor is null ? $"#{a.DoctorId}" : doctor.Name.ToString(),
            Status = a.Status.ToString()
        };
    }

    /// <summary>Searches patients by name and/or medical card fragments.</summary>
    public IReadOnlyCollection<Patient> SearchPatients(string? namePart, string? cardPart)
    {
        var query = _patients.GetAll().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(namePart))
        {
            var normalizedName = namePart.Trim();
            query = query.Where(p =>
                p.Name.FirstName.Contains(normalizedName, StringComparison.OrdinalIgnoreCase) ||
                p.Name.LastName.Contains(normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(cardPart))
        {
            var normalizedCard = cardPart.Trim();
            query = query.Where(p => p.MedicalCardNumber.Contains(normalizedCard, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(p => p.Name.LastName)
            .ThenBy(p => p.Name.FirstName)
            .ToList();
    }

    /// <summary>Returns top doctors by appointment count.</summary>
    public IReadOnlyCollection<DoctorAppointmentStat> GetTopDoctorsByAppointments(int top)
    {
        var limit = Math.Max(top, 0);
        var counts = _appointments.GetAll()
            .GroupBy(a => a.DoctorId)
            .Select(group => new { DoctorId = group.Key, Count = group.Count() })
            .OrderByDescending(item => item.Count)
            .Take(limit)
            .ToList();

        var doctorsById = _doctors.GetAll().ToDictionary(d => d.Id);

        var result = new List<DoctorAppointmentStat>();
        foreach (var item in counts)
        {
            if (!doctorsById.TryGetValue(item.DoctorId, out var doctor))
                continue;

            result.Add(new DoctorAppointmentStat
            {
                Doctor = doctor,
                Count = item.Count
            });
        }

        return result;
    }

    /// <summary>Builds aggregated appointment statistics.</summary>
    public AppointmentStats GetAppointmentStats()
    {
        var appointments = _appointments.GetAll();
        var now = _clock.Now;

        return new AppointmentStats
        {
            Total = appointments.Count,
            NewCount = appointments.Count(a => a.Status == AppointmentStatus.New),
            ConfirmedCount = appointments.Count(a => a.Status == AppointmentStatus.Confirmed),
            CancelledCount = appointments.Count(a => a.Status == AppointmentStatus.Cancelled),
            UpcomingCount = appointments.Count(a => a.AppointmentTime > now && a.Status != AppointmentStatus.Cancelled)
        };
    }
}
