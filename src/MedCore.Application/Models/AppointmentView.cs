namespace MedCore.Application.Models;

public class AppointmentView
{
    public int Id { get; init; }
    public DateTime AppointmentTime { get; init; }
    public string PatientName { get; init; } = "";
    public string DoctorName { get; init; } = "";
    public string Status { get; init; } = "";
}
