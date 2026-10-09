using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class CarRepository
{
    private readonly ContextDb _db;

    public CarRepository(ContextDb db) => _db = db;

    public async Task<List<Car>> GetAllAsync()
    {
        return await _db.Cars
            .Include(c => c.Client)
            .OrderBy(c => c.Brand)
            .ThenBy(c => c.Model)
            .ToListAsync();
    }

    public async Task AddAsync(Car car)
    {
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();
    }
}
