using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
namespace MedCore.Infrastructure.Repositories;
public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _storage = new();

    public void Add(Appointment app)
    {
        _storage.Add(app);
    }

    public List<Appointment> GetAll()
    {
        return _storage.ToList();
    }

    public List<Appointment> GetByDoctorId(int id)
    {
        return _storage.Where(a => a.DoctorId == id).ToList();
    }
}