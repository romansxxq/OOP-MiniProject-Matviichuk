using MedCore.Domain.Entities;
using MedCore.Domain.ValueObjects;
using MedCore.Application.Factories;
using MedCore.Application.Services;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Strategies;

Console.WriteLine("=== MedCore: Запуск Ітерації 1 ===\n");

var repo = new InMemoryAppointmentRepository();
var strategy = new TimeSlotValidationStrategy();
var service = new AppointmentService(repo, strategy);

StaffFactory factory = new DoctorFactory();
var doctor = (Doctor)factory.CreateStaff("Іван Петренко", "Хірург");
doctor.PerformDuty();

var patient = new Patient(1, new FullName("Олена", "Коваленко"), "MH-999");

DateTime time = DateTime.Now.AddDays(1);

var res1 = service.CreateAppointment(patient.Id, doctor.Id, time);
Console.WriteLine($"Спроба 1: {res1.Message}");

var res2 = service.CreateAppointment(2, doctor.Id, time); // Конфлікт, інший пацієнт
Console.WriteLine($"Спроба 2: {res2.Message}");

Console.ReadLine();