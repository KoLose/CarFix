using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class RoleRepository
{
    private readonly ContextDb _db;

    public RoleRepository(ContextDb db) => _db = db;

    public async Task<List<Role>> GetAllAsync()
    {
        return await _db.Roles.OrderBy(r => r.Id).ToListAsync();
    }
}
