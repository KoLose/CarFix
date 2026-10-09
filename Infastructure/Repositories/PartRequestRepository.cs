using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class PartRequestRepository
{
    private readonly ContextDb _db;

    public PartRequestRepository(ContextDb db) => _db = db;

    public async Task AddAsync(PartRequest request)
    {
        _db.PartRequests.Add(request);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PartRequest>> GetAllAsync()
    {
        return await _db.PartRequests
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<PartRequest>> GetByUserAsync(int userId)
    {
        return await _db.PartRequests
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
