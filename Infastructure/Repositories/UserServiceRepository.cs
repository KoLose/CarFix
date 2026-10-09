using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class UserServiceRepository
{
    private readonly ContextDb _db;

    public UserServiceRepository(ContextDb db) => _db = db;

    public async Task<List<int>> GetServiceIdsForUserAsync(int userId)
    {
        return await _db.UserServices
            .Where(us => us.UserId == userId)
            .Select(us => us.ServiceId)
            .ToListAsync();
    }

    public async Task<bool> HasAllServicesAsync(int userId, IEnumerable<int> serviceIds)
    {
        var required = serviceIds.Distinct().ToList();
        if (required.Count == 0) return true;

        var owned = await GetServiceIdsForUserAsync(userId);
        return required.All(id => owned.Contains(id));
    }

    public async Task<List<User>> GetMechanicsWithServicesAsync(IEnumerable<int> serviceIds)
    {
        var required = serviceIds.Distinct().ToList();
        var mechanics = await _db.Users
            .Include(u => u.UserServices)
            .Where(u => u.RoleId == RoleIds.Mechanic)
            .ToListAsync();

        if (required.Count == 0) return mechanics;

        return mechanics
            .Where(m => required.All(id => m.UserServices.Any(us => us.ServiceId == id)))
            .ToList();
    }
}
