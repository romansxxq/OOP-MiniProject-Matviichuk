using MedCore.Domain.Entities;

namespace MedCore.Application.Models;

public class DoctorAppointmentStat
{
    public Doctor Doctor { get; init; } = null!;
    public int Count { get; init; }
}
