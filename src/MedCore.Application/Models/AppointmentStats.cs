namespace MedCore.Application.Models;

public class AppointmentStats
{
    public int Total { get; init; }
    public int NewCount { get; init; }
    public int ConfirmedCount { get; init; }
    public int CancelledCount { get; init; }
    public int UpcomingCount { get; init; }
}
