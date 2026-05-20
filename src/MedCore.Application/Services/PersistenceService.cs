using System.Text.Json;
using MedCore.Application.Common;
using MedCore.Application.Models;
using MedCore.Domain.Abstractions;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Services;

public class PersistenceService
{
    private readonly IDataStore<MedCoreData> _store;
    private readonly IRepository<Patient, int> _patients;
    private readonly IRepository<Doctor, int> _doctors;
    private readonly IRepository<Nurse, int> _nurses;
    private readonly IRepository<Department, int> _departments;
    private readonly IAppointmentRepository _appointments;
    private readonly IErrorReporter _errorReporter;

    public PersistenceService(
        IDataStore<MedCoreData> store,
        IRepository<Patient, int> patients,
        IRepository<Doctor, int> doctors,
        IRepository<Nurse, int> nurses,
        IRepository<Department, int> departments,
        IAppointmentRepository appointments,
        IErrorReporter? errorReporter = null)
    {
        _store = store;
        _patients = patients;
        _doctors = doctors;
        _nurses = nurses;
        _departments = departments;
        _appointments = appointments;
        _errorReporter = errorReporter ?? new NullErrorReporter();
    }

    public async Task<Result> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await _store.LoadAsync(cancellationToken);
            var validation = ValidateData(data);
            if (!validation.IsSuccess)
                return validation;

            ReplaceAll(_patients, data.Patients);
            ReplaceAll(_doctors, data.Doctors);
            ReplaceAll(_nurses, data.Nurses);
            ReplaceAll(_departments, data.Departments);
            ReplaceAll(_appointments, data.Appointments);

            return Result.Success("Дані завантажено.");
        }
        catch (FileNotFoundException)
        {
            _errorReporter.Report("Data file not found during load.");
            return Result.Success("Файл даних не знайдено. Стартуємо з порожнього стану.");
        }
        catch (JsonException ex)
        {
            _errorReporter.Report("Invalid JSON while loading data.", ex);
            return Result.Failure($"Помилка JSON: {ex.Message}");
        }
        catch (IOException ex)
        {
            _errorReporter.Report("I/O error while loading data.", ex);
            return Result.Failure($"Помилка читання файлу: {ex.Message}");
        }
    }

    public async Task<Result> SaveAsync(CancellationToken cancellationToken = default)
    {
        var data = new MedCoreData
        {
            Patients = _patients.GetAll().ToList(),
            Doctors = _doctors.GetAll().ToList(),
            Nurses = _nurses.GetAll().ToList(),
            Departments = _departments.GetAll().ToList(),
            Appointments = _appointments.GetAll().ToList()
        };

        try
        {
            await _store.SaveAsync(data, cancellationToken);
            return Result.Success("Дані збережено.");
        }
        catch (IOException ex)
        {
            _errorReporter.Report("I/O error while saving data.", ex);
            return Result.Failure($"Помилка запису файлу: {ex.Message}");
        }
    }

    private static Result ValidateData(MedCoreData data)
    {
        if (HasDuplicateIds(data.Patients.Select(p => p.Id)))
            return Result.Failure("Конфлікт даних: дублікати ID пацієнтів.");

        if (HasDuplicateIds(data.Doctors.Select(d => d.Id)))
            return Result.Failure("Конфлікт даних: дублікати ID лікарів.");

        if (HasDuplicateIds(data.Nurses.Select(n => n.Id)))
            return Result.Failure("Конфлікт даних: дублікати ID медсестер.");

        if (HasDuplicateIds(data.Departments.Select(d => d.Id)))
            return Result.Failure("Конфлікт даних: дублікати ID відділень.");

        if (HasDuplicateIds(data.Appointments.Select(a => a.Id)))
            return Result.Failure("Конфлікт даних: дублікати ID записів.");

        var patientIds = data.Patients.Select(p => p.Id).ToHashSet();
        var doctorIds = data.Doctors.Select(d => d.Id).ToHashSet();
        var departmentIds = data.Departments.Select(d => d.Id).ToHashSet();

        if (data.Appointments.Any(a => !patientIds.Contains(a.PatientId) || !doctorIds.Contains(a.DoctorId)))
            return Result.Failure("Конфлікт даних: у записах є невідомі пацієнти або лікарі.");

        if (data.Doctors.Any(d => d.DepartmentId.HasValue && !departmentIds.Contains(d.DepartmentId.Value)))
            return Result.Failure("Конфлікт даних: лікар має неіснуюче відділення.");

        return Result.Success("Дані валідні.");
    }

    private static bool HasDuplicateIds(IEnumerable<int> ids)
    {
        var seen = new HashSet<int>();
        foreach (var id in ids)
        {
            if (!seen.Add(id))
                return true;
        }

        return false;
    }

    private static void ReplaceAll<T>(IRepository<T, int> repository, IEnumerable<T> items) where T : IEntity<int>
    {
        foreach (var existing in repository.GetAll().ToList())
        {
            repository.Delete(existing.Id);
        }

        foreach (var item in items)
        {
            repository.Add(item);
        }
    }
}
