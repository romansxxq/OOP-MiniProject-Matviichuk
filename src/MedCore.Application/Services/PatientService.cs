using MedCore.Application.Common;
using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
using MedCore.Domain.ValueObjects;

namespace MedCore.Application.Services;

public class PatientService
{
    private readonly IRepository<Patient, int> _patients;

    public PatientService(IRepository<Patient, int> patients)
    {
        _patients = patients;
    }

    public Result<Patient> RegisterPatient(string firstName, string lastName, string medicalCardNumber)
    {
        if (string.IsNullOrWhiteSpace(medicalCardNumber))
            return Result<Patient>.Failure("Номер медичної картки не може бути порожнім.");

        var normalizedCard = medicalCardNumber.Trim();
        var alreadyExists = _patients.GetAll()
            .Any(p => p.MedicalCardNumber.Equals(normalizedCard, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
            return Result<Patient>.Failure("Пацієнт з таким номером картки вже існує.");

        try
        {
            var id = IdGenerator.NextId(_patients);
            var patient = new Patient(id, new FullName(firstName, lastName), normalizedCard);
            _patients.Add(patient);
            return Result<Patient>.Success(patient, "Пацієнта зареєстровано.");
        }
        catch (ArgumentException ex)
        {
            return Result<Patient>.Failure(ex.Message);
        }
    }
}
