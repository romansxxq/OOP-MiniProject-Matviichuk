using MedCore.Application.Services;
using MedCore.Domain.Entities;
using MedCore.Infrastructure.Repositories;
using Xunit;

namespace MedCore.Tests;

public class PatientServiceTests
{
    [Fact]
    public void RegisterPatient_ShouldCreatePatient_WhenDataValid()
    {
        var repo = new InMemoryRepository<Patient, int>();
        var service = new PatientService(repo);

        var result = service.RegisterPatient("Iryna", "Melnyk", "CARD-01");

        Assert.True(result.IsSuccess);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void RegisterPatient_ShouldRejectDuplicateMedicalCardNumber()
    {
        var repo = new InMemoryRepository<Patient, int>();
        var service = new PatientService(repo);

        service.RegisterPatient("Iryna", "Melnyk", "CARD-01");
        var result = service.RegisterPatient("Olena", "Koval", "CARD-01");

        Assert.False(result.IsSuccess);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void RegisterPatient_ShouldRejectEmptyCardNumber()
    {
        var repo = new InMemoryRepository<Patient, int>();
        var service = new PatientService(repo);

        var result = service.RegisterPatient("Iryna", "Melnyk", " ");

        Assert.False(result.IsSuccess);
        Assert.Empty(repo.GetAll());
    }
}
