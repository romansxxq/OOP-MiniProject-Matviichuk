using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Domain.Interfaces;

namespace MedCore.Infrastructure.Strategies;

public class DoctorDailyLimitValidationStrategy : IValidationStrategy
{
    private readonly int _dailyLimit;

    public DoctorDailyLimitValidationStrategy(int dailyLimit)
    {
        if (dailyLimit <= 0) throw new ArgumentException("Daily limit must be positive.", nameof(dailyLimit));
        _dailyLimit = dailyLimit;
    }

    public bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing)
    {
        var sameDayCount = existing
            .Count(a => a.DoctorId == app.DoctorId &&
                        a.AppointmentTime.Date == app.AppointmentTime.Date &&
                        a.Status != AppointmentStatus.Cancelled);

        return sameDayCount < _dailyLimit;
    }
}
