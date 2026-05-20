using MedCore.Application.Common;
using MedCore.Domain.Entities;
using MedCore.Domain.ValueObjects;
using MedCore.Infrastructure.Repositories;
using Xunit;

namespace MedCore.Tests;

public class RepositoryTests
{
    [Fact]
    public void Add_ShouldThrow_WhenDuplicateId()
    {
        var repo = new InMemoryRepository<Patient, int>();
        var first = new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01");
        var second = new Patient(1, new FullName("Ivan", "Petrenko"), "MC-02");

        repo.Add(first);

        Assert.Throws<InvalidOperationException>(() => repo.Add(second));
    }

    [Fact]
    public void Update_ShouldThrow_WhenMissing()
    {
        var repo = new InMemoryRepository<Patient, int>();
        var patient = new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01");

        Assert.Throws<InvalidOperationException>(() => repo.Update(patient));
    }

    [Fact]
    public void Delete_ShouldRemoveEntity()
    {
        var repo = new InMemoryRepository<Patient, int>();
        var patient = new Patient(1, new FullName("Olena", "Kovalenko"), "MC-01");

        repo.Add(patient);
        repo.Delete(1);

        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void IdGenerator_ShouldReturnMaxPlusOne()
    {
        var repo = new InMemoryRepository<Department, int>();
        repo.Add(new Department(1, "Therapy", 1));
        repo.Add(new Department(4, "Surgery", 2));

        var next = IdGenerator.NextId(repo);

        Assert.Equal(5, next);
    }
}
