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
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

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

var jsonOptions = new System.Text.Json.JsonSerializerOptions
{
	WriteIndented = true,
	PropertyNameCaseInsensitive = true
};
var dataStore = new JsonFileDataStore(Path.Combine("data", "medcore.json"), jsonOptions);
var errorReporter = new ConsoleErrorReporter();
var persistenceService = new PersistenceService(dataStore, patientRepo, doctorRepo, nurseRepo, departmentRepo, appointmentRepo, errorReporter);

var loadResult = await persistenceService.LoadAsync();
ConsoleOutput.ShowResult(loadResult);

var mainMenuOptions = new[]
{
	"Реєстрація пацієнта",
	"Додати лікаря",
	"Створити відділення",
	"Призначити лікаря до відділення",
	"Створити запис на прийом",
	"Підтвердити запис",
	"Скасувати запис",
	"Запити та аналітика",
	"Зберегти дані",
	"Вийти"
};

while (true)
{
	var choice = ConsoleInput.ReadMenu("Головне меню", mainMenuOptions);

	switch (choice)
	{
		case "Реєстрація пацієнта":
			RegisterPatient();
			break;
		case "Додати лікаря":
			RegisterDoctor();
			break;
		case "Створити відділення":
			CreateDepartment();
			break;
		case "Призначити лікаря до відділення":
			AssignDoctor();
			break;
		case "Створити запис на прийом":
			CreateAppointment();
			break;
		case "Підтвердити запис":
			ConfirmAppointment();
			break;
		case "Скасувати запис":
			CancelAppointment();
			break;
		case "Запити та аналітика":
			HandleQueries();
			break;
		case "Зберегти дані":
			SaveData();
			break;
		case "Вийти":
			return;
	}
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
	var queryMenuOptions = new[]
	{
		"Активні записи",
		"Розклад лікаря",
		"Пошук пацієнтів",
		"Топ лікарів за кількістю записів",
		"Статистика записів",
		"Назад"
	};

	while (true)
	{
		var choice = ConsoleInput.ReadMenu("Запити та аналітика", queryMenuOptions);
		switch (choice)
		{
			case "Активні записи":
				var active = queryService.GetActiveAppointments();
				ConsoleOutput.PrintAppointments(active, patientRepo, doctorRepo);
				break;
			case "Розклад лікаря":
				var doctorId = ConsoleInput.ReadInt("ID лікаря: ", "Введіть коректне число.");
				var schedule = queryService.GetDoctorAppointments(doctorId);
				ConsoleOutput.PrintAppointments(schedule, patientRepo, doctorRepo);
				break;
			case "Пошук пацієнтів":
				var name = ConsoleInput.ReadOptionalText("Фрагмент імені (опціонально): ");
				var card = ConsoleInput.ReadOptionalText("Фрагмент картки (опціонально): ");
				var patients = queryService.SearchPatients(name, card);
				ConsoleOutput.PrintPatients(patients);
				break;
			case "Топ лікарів за кількістю записів":
				var top = ConsoleInput.ReadInt("Скільки показати: ", "Введіть коректне число.");
				var topDoctors = queryService.GetTopDoctorsByAppointments(top);
				ConsoleOutput.PrintTopDoctors(topDoctors);
				break;
			case "Статистика записів":
				var stats = queryService.GetAppointmentStats();
				ConsoleOutput.PrintAppointmentStats(stats);
				break;
			case "Назад":
				return;
		}
	}
}
