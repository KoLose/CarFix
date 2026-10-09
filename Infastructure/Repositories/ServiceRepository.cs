using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class ServiceRepository
{
    private readonly ContextDb _db;

    public ServiceRepository(ContextDb db) => _db = db;

    public async Task<List<Service>> GetAllAsync()
    {
        return await _db.Services.OrderBy(s => s.Name).ToListAsync();
    }
}
