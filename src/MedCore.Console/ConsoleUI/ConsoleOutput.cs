using MedCore.Application.Common;
using MedCore.Application.Models;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
using Spectre.Console;

namespace MedCore.ConsoleUI;

public static class ConsoleOutput
{
    public static void ShowResult(Result result)
    {
        var color = result.IsSuccess ? "green" : "red";
        AnsiConsole.MarkupLine($"[{color}]{Markup.Escape(result.Message)}[/]");
    }

    public static void PrintAppointments(
        IEnumerable<Appointment> appointments,
        IRepository<Patient, int> patientRepo,
        IRepository<Doctor, int> doctorRepo)
    {
        var list = appointments.ToList();
        if (list.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Немає записів.[/]");
            return;
        }
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Записи на прийом");
        table.AddColumn("ID");
        table.AddColumn("Дата і час");
        table.AddColumn("Пацієнт");
        table.AddColumn("Лікар");
        table.AddColumn("Статус");

        foreach (var appointment in list)
        {
            var patient = patientRepo.GetById(appointment.PatientId);
            var doctor = doctorRepo.GetById(appointment.DoctorId);
            var patientName = patient is null ? $"#{appointment.PatientId}" : patient.Name.ToString();
            var doctorName = doctor is null ? $"#{appointment.DoctorId}" : doctor.Name.ToString();

            table.AddRow(
                appointment.Id.ToString(),
                appointment.AppointmentTime.ToString("g"),
                Markup.Escape(patientName),
                Markup.Escape(doctorName),
                Markup.Escape(appointment.Status.ToString()));
        }

        AnsiConsole.Write(table);
    }

    public static void PrintPatients(IEnumerable<Patient> patients)
    {
        var list = patients.ToList();
        if (list.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Нічого не знайдено.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Пацієнти");
        table.AddColumn("ID");
        table.AddColumn("ПІБ");
        table.AddColumn("Картка");

        foreach (var patient in list)
        {
            table.AddRow(
                patient.Id.ToString(),
                Markup.Escape(patient.Name.ToString()),
                Markup.Escape(patient.MedicalCardNumber));
        }

        AnsiConsole.Write(table);
    }

    public static void PrintTopDoctors(IEnumerable<DoctorAppointmentStat> stats)
    {
        var list = stats.ToList();
        if (list.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Немає даних.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Топ лікарів");
        table.AddColumn("ID");
        table.AddColumn("Лікар");
        table.AddColumn("Записів");

        foreach (var item in list)
        {
            table.AddRow(
                item.Doctor.Id.ToString(),
                Markup.Escape(item.Doctor.Name.ToString()),
                item.Count.ToString());
        }

        AnsiConsole.Write(table);
    }

    public static void PrintAppointmentStats(AppointmentStats stats)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Статистика записів");
        table.AddColumn("Показник");
        table.AddColumn("Значення");

        table.AddRow("Всього", stats.Total.ToString());
        table.AddRow("Нові", stats.NewCount.ToString());
        table.AddRow("Підтверджені", stats.ConfirmedCount.ToString());
        table.AddRow("Скасовані", stats.CancelledCount.ToString());
        table.AddRow("Майбутні активні", stats.UpcomingCount.ToString());

        AnsiConsole.Write(table);
    }
}
