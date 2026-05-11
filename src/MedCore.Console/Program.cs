using MedCore.Application.Factories;
using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Strategies;
using MedCore.Infrastructure.Stores;

Console.WriteLine("=== MedCore: Ітерація 2 ===\n");

var appointmentRepo = new InMemoryAppointmentRepository();
var patientRepo = new InMemoryRepository<Patient, int>();
var doctorRepo = new InMemoryRepository<Doctor, int>();
var nurseRepo = new InMemoryRepository<Nurse, int>();
var departmentRepo = new InMemoryRepository<Department, int>();

IValidationStrategy validationStrategy = new CompositeValidationStrategy(new IValidationStrategy[]
{
	new TimeSlotValidationStrategy(),
	new PatientTimeConflictValidationStrategy(),
	new DoctorDailyLimitValidationStrategy(10)
});

var appointmentService = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, validationStrategy);
var patientService = new PatientService(patientRepo);
var doctorService = new DoctorService(doctorRepo, new DoctorFactory());
var departmentService = new DepartmentService(departmentRepo, doctorRepo);
var queryService = new QueryService(appointmentRepo, patientRepo, doctorRepo);

var dataStore = new JsonFileDataStore(Path.Combine("data", "medcore.json"));
var persistenceService = new PersistenceService(dataStore, patientRepo, doctorRepo, nurseRepo, departmentRepo, appointmentRepo);

var loadResult = await persistenceService.LoadAsync();
Console.WriteLine(loadResult.Message);

while (true)
{
	PrintMenu();
	var choice = Console.ReadLine();

	switch (choice)
	{
		case "1":
			RegisterPatient();
			break;
		case "2":
			RegisterDoctor();
			break;
		case "3":
			CreateDepartment();
			break;
		case "4":
			AssignDoctor();
			break;
		case "5":
			CreateAppointment();
			break;
		case "6":
			ConfirmAppointment();
			break;
		case "7":
			CancelAppointment();
			break;
		case "8":
			HandleQueries();
			break;
		case "9":
			SaveData();
			break;
		case "0":
			return;
		default:
			Console.WriteLine("Невірний вибір.");
			break;
	}
}

void PrintMenu()
{
	Console.WriteLine();
	Console.WriteLine("1. Реєстрація пацієнта");
	Console.WriteLine("2. Додати лікаря");
	Console.WriteLine("3. Створити відділення");
	Console.WriteLine("4. Призначити лікаря до відділення");
	Console.WriteLine("5. Створити запис на прийом");
	Console.WriteLine("6. Підтвердити запис");
	Console.WriteLine("7. Скасувати запис");
	Console.WriteLine("8. Запити та аналітика");
	Console.WriteLine("9. Зберегти дані");
	Console.WriteLine("0. Вийти");
	Console.Write("Ваш вибір: ");
}

void RegisterPatient()
{
	var firstName = ReadText("Ім'я: ");
	var lastName = ReadText("Прізвище: ");
	var cardNumber = ReadText("Номер медичної картки: ");

	var result = patientService.RegisterPatient(firstName, lastName, cardNumber);
	ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID пацієнта: {result.Value.Id}");
}

void RegisterDoctor()
{
	var fullName = ReadText("Повне ім'я: ");
	var specialization = ReadText("Спеціалізація: ");

	var result = doctorService.RegisterDoctor(fullName, specialization);
	ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID лікаря: {result.Value.Id}");
}

void CreateDepartment()
{
	var name = ReadText("Назва відділення: ");
	var floor = ReadInt("Поверх: ");

	var result = departmentService.CreateDepartment(name, floor);
	ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID відділення: {result.Value.Id}");
}

void AssignDoctor()
{
	var doctorId = ReadInt("ID лікаря: ");
	var departmentId = ReadInt("ID відділення: ");

	var result = departmentService.AssignDoctorToDepartment(doctorId, departmentId);
	ShowResult(result);
}

void CreateAppointment()
{
	var patientId = ReadInt("ID пацієнта: ");
	var doctorId = ReadInt("ID лікаря: ");
	var time = ReadDateTime("Дата і час (наприклад 2026-05-11 14:30): ");

	var result = appointmentService.CreateAppointment(patientId, doctorId, time);
	ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID запису: {result.Value.Id}");
}

void ConfirmAppointment()
{
	var appointmentId = ReadInt("ID запису: ");
	ShowResult(appointmentService.ConfirmAppointment(appointmentId));
}

void CancelAppointment()
{
	var appointmentId = ReadInt("ID запису: ");
	ShowResult(appointmentService.CancelAppointment(appointmentId));
}

void SaveData()
{
	var result = persistenceService.SaveAsync().GetAwaiter().GetResult();
	ShowResult(result);
}

void HandleQueries()
{
	while (true)
	{
		Console.WriteLine();
		Console.WriteLine("1. Активні записи");
		Console.WriteLine("2. Розклад лікаря");
		Console.WriteLine("3. Пошук пацієнтів");
		Console.WriteLine("4. Топ лікарів за кількістю записів");
		Console.WriteLine("5. Статистика записів");
		Console.WriteLine("0. Назад");
		Console.Write("Ваш вибір: ");

		var choice = Console.ReadLine();
		switch (choice)
		{
			case "1":
				var active = queryService.GetActiveAppointments();
				PrintAppointments(active);
				break;
			case "2":
				var doctorId = ReadInt("ID лікаря: ");
				var schedule = queryService.GetDoctorAppointments(doctorId);
				PrintAppointments(schedule);
				break;
			case "3":
				var name = ReadOptionalText("Фрагмент імені (опціонально): ");
				var card = ReadOptionalText("Фрагмент картки (опціонально): ");
				var patients = queryService.SearchPatients(name, card);
				foreach (var patient in patients)
				{
					Console.WriteLine($"{patient.Id}: {patient.Name} | {patient.MedicalCardNumber}");
				}
				break;
			case "4":
				var top = ReadInt("Скільки показати: ");
				var topDoctors = queryService.GetTopDoctorsByAppointments(top);
				foreach (var item in topDoctors)
				{
					Console.WriteLine($"{item.Doctor.Id}: {item.Doctor.Name} | записів: {item.Count}");
				}
				break;
			case "5":
				var stats = queryService.GetAppointmentStats();
				Console.WriteLine($"Всього: {stats.Total}");
				Console.WriteLine($"Нові: {stats.NewCount}");
				Console.WriteLine($"Підтверджені: {stats.ConfirmedCount}");
				Console.WriteLine($"Скасовані: {stats.CancelledCount}");
				Console.WriteLine($"Майбутні активні: {stats.UpcomingCount}");
				break;
			case "0":
				return;
			default:
				Console.WriteLine("Невірний вибір.");
				break;
		}
	}
}

void PrintAppointments(IEnumerable<Appointment> appointments)
{
	foreach (var appointment in appointments)
	{
		var patient = patientRepo.GetById(appointment.PatientId);
		var doctor = doctorRepo.GetById(appointment.DoctorId);
		var patientName = patient is null ? $"#{appointment.PatientId}" : patient.Name.ToString();
		var doctorName = doctor is null ? $"#{appointment.DoctorId}" : doctor.Name.ToString();

		Console.WriteLine($"{appointment.Id}: {appointment.AppointmentTime:g} | {patientName} -> {doctorName} | {appointment.Status}");
	}
}

void ShowResult(MedCore.Application.Common.Result result)
{
	Console.WriteLine(result.Message);
}

int ReadInt(string prompt)
{
	while (true)
	{
		Console.Write(prompt);
		var input = Console.ReadLine();
		if (int.TryParse(input, out var value))
			return value;

		Console.WriteLine("Введіть коректне число.");
	}
}

DateTime ReadDateTime(string prompt)
{
	while (true)
	{
		Console.Write(prompt);
		var input = Console.ReadLine();
		if (DateTime.TryParse(input, out var value))
			return value;

		Console.WriteLine("Некоректний формат дати.");
	}
}

string ReadText(string prompt)
{
	while (true)
	{
		Console.Write(prompt);
		var input = Console.ReadLine();
		if (!string.IsNullOrWhiteSpace(input))
			return input.Trim();

		Console.WriteLine("Поле не може бути порожнім.");
	}
}

string? ReadOptionalText(string prompt)
{
	Console.Write(prompt);
	var input = Console.ReadLine();
	return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
}