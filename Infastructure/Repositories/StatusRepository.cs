using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class StatusRepository
{
    private readonly ContextDb _db;

    public StatusRepository(ContextDb db) => _db = db;

    public async Task<List<Status>> GetAllAsync()
    {
        return await _db.Statuses.OrderBy(s => s.Id).ToListAsync();
    }
}
