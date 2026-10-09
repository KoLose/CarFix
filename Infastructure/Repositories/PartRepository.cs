using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class PartRepository
{
    private readonly ContextDb _db;

    public PartRepository(ContextDb db) => _db = db;

    public async Task<List<Part>> GetAllAsync()
    {
        return await _db.Parts
            .Include(p => p.Supplier)
            .OrderBy(p => p.Article)
            .ToListAsync();
    }

    public async Task<Part?> GetByIdAsync(int id) => await _db.Parts.FindAsync(id);

    public async Task UpdateAsync(Part part)
    {
        _db.Parts.Update(part);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> HasEnoughAsync(int partId, int quantity)
    {
        var part = await _db.Parts.FindAsync(partId);
        return part != null && part.Quantity >= quantity;
    }

    public async Task<decimal> GetTotalPurchaseCostAsync()
    {
        return await _db.Parts.SumAsync(p => p.PurchasePrice * p.Quantity);
    }
}
