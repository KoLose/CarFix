using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class SupplierRepository
{
    private readonly ContextDb _db;

    public SupplierRepository(ContextDb db) => _db = db;

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _db.Suppliers.OrderBy(s => s.Name).ToListAsync();
    }
}
