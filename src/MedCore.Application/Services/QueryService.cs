using MedCore.Application.Common;
using MedCore.Application.Models;
using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;

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

    public IReadOnlyCollection<Appointment> GetActiveAppointments()
    {
        return _appointments.GetAll()
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentTime)
            .ToList();
    }

    public IReadOnlyCollection<Appointment> GetDoctorAppointments(int doctorId)
    {
        return _appointments.GetAll()
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.AppointmentTime)
            .ToList();
    }

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

        return counts
            .Where(item => doctorsById.ContainsKey(item.DoctorId))
            .Select(item => new DoctorAppointmentStat
            {
                Doctor = doctorsById[item.DoctorId],
                Count = item.Count
            })
            .ToList();
    }

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
