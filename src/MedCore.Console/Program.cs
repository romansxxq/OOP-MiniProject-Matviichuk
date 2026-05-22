using MedCore.Application.Common;
using MedCore.Application.Factories;
using MedCore.Application.Services;
using MedCore.Console;
using MedCore.ConsoleUI;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
using MedCore.Infrastructure.Repositories;
using MedCore.Infrastructure.Strategies;
using MedCore.Infrastructure.Stores;

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

var clock = new SystemClock();
var appointmentService = new AppointmentService(appointmentRepo, patientRepo, doctorRepo, validationStrategy, clock);
var patientService = new PatientService(patientRepo);
var doctorService = new DoctorService(doctorRepo, new DoctorFactory());
var departmentService = new DepartmentService(departmentRepo, doctorRepo);
var queryService = new QueryService(appointmentRepo, patientRepo, doctorRepo, clock);

var dataStore = new JsonFileDataStore(Path.Combine("data", "medcore.json"));
var errorReporter = new ConsoleErrorReporter();
var persistenceService = new PersistenceService(dataStore, patientRepo, doctorRepo, nurseRepo, departmentRepo, appointmentRepo, errorReporter);

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
	var firstName = ConsoleInput.ReadText("Ім'я: ", "Поле не може бути порожнім.");
	var lastName = ConsoleInput.ReadText("Прізвище: ", "Поле не може бути порожнім.");
	var cardNumber = ConsoleInput.ReadText("Номер медичної картки: ", "Поле не може бути порожнім.");

	var result = patientService.RegisterPatient(firstName, lastName, cardNumber);
	ConsoleOutput.ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID пацієнта: {result.Value.Id}");
}

void RegisterDoctor()
{
	var fullName = ConsoleInput.ReadText("Повне ім'я: ", "Поле не може бути порожнім.");
	var specialization = ConsoleInput.ReadText("Спеціалізація: ", "Поле не може бути порожнім.");

	var result = doctorService.RegisterDoctor(fullName, specialization);
	ConsoleOutput.ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID лікаря: {result.Value.Id}");
}

void CreateDepartment()
{
	var name = ConsoleInput.ReadText("Назва відділення: ", "Поле не може бути порожнім.");
	var floor = ConsoleInput.ReadInt("Поверх: ", "Введіть коректне число.");

	var result = departmentService.CreateDepartment(name, floor);
	ConsoleOutput.ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID відділення: {result.Value.Id}");
}

void AssignDoctor()
{
	var doctorId = ConsoleInput.ReadInt("ID лікаря: ", "Введіть коректне число.");
	var departmentId = ConsoleInput.ReadInt("ID відділення: ", "Введіть коректне число.");

	var result = departmentService.AssignDoctorToDepartment(doctorId, departmentId);
	ConsoleOutput.ShowResult(result);
}

void CreateAppointment()
{
	var patientId = ConsoleInput.ReadInt("ID пацієнта: ", "Введіть коректне число.");
	var doctorId = ConsoleInput.ReadInt("ID лікаря: ", "Введіть коректне число.");
	var time = ConsoleInput.ReadDateTime("Дата і час (наприклад 2026-05-11 14:30): ", "Некоректний формат дати.");

	var result = appointmentService.CreateAppointment(patientId, doctorId, time);
	ConsoleOutput.ShowResult(result);
	if (result.IsSuccess && result.Value is not null)
		Console.WriteLine($"ID запису: {result.Value.Id}");
}

void ConfirmAppointment()
{
	var appointmentId = ConsoleInput.ReadInt("ID запису: ", "Введіть коректне число.");
	ConsoleOutput.ShowResult(appointmentService.ConfirmAppointment(appointmentId));
}

void CancelAppointment()
{
	var appointmentId = ConsoleInput.ReadInt("ID запису: ", "Введіть коректне число.");
	ConsoleOutput.ShowResult(appointmentService.CancelAppointment(appointmentId));
}

void SaveData()
{
	var result = persistenceService.SaveAsync().GetAwaiter().GetResult();
	ConsoleOutput.ShowResult(result);
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
				ConsoleOutput.PrintAppointments(active, patientRepo, doctorRepo);
				break;
			case "2":
				var doctorId = ConsoleInput.ReadInt("ID лікаря: ", "Введіть коректне число.");
				var schedule = queryService.GetDoctorAppointments(doctorId);
				ConsoleOutput.PrintAppointments(schedule, patientRepo, doctorRepo);
				break;
			case "3":
				var name = ConsoleInput.ReadOptionalText("Фрагмент імені (опціонально): ");
				var card = ConsoleInput.ReadOptionalText("Фрагмент картки (опціонально): ");
				var patients = queryService.SearchPatients(name, card);
				foreach (var patient in patients)
				{
					Console.WriteLine($"{patient.Id}: {patient.Name} | {patient.MedicalCardNumber}");
				}
				break;
			case "4":
				var top = ConsoleInput.ReadInt("Скільки показати: ", "Введіть коректне число.");
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
