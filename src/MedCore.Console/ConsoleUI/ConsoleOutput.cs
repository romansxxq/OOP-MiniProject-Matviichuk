using MedCore.Application.Common;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.ConsoleUI;

public static class ConsoleOutput
{
    public static void ShowResult(Result result)
    {
        System.Console.WriteLine(result.Message);
    }

    public static void PrintAppointments(
        IEnumerable<Appointment> appointments,
        IRepository<Patient, int> patientRepo,
        IRepository<Doctor, int> doctorRepo)
    {
        foreach (var appointment in appointments)
        {
            var patient = patientRepo.GetById(appointment.PatientId);
            var doctor = doctorRepo.GetById(appointment.DoctorId);
            var patientName = patient is null ? $"#{appointment.PatientId}" : patient.Name.ToString();
            var doctorName = doctor is null ? $"#{appointment.DoctorId}" : doctor.Name.ToString();

            System.Console.WriteLine($"{appointment.Id}: {appointment.AppointmentTime:g} | {patientName} -> {doctorName} | {appointment.Status}");
        }
    }
}
