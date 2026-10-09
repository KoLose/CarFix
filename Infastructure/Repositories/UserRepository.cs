using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class UserRepository
{
    private readonly ContextDb _db;

    public UserRepository(ContextDb db) => _db = db;

    public async Task<User?> GetByCredentialsAsync(string login, string password)
    {
        return await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<User>> GetByRoleAsync(int roleId)
    {
        return await _db.Users.Where(u => u.RoleId == roleId).OrderBy(u => u.Name).ToListAsync();
    }

    public async Task<List<User>> GetClientsAsync() => await GetByRoleAsync(RoleIds.Client);

    public async Task<List<User>> GetMechanicsAsync() => await GetByRoleAsync(RoleIds.Mechanic);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> HasActiveOrdersAsync(int mechanicId)
    {
        return await _db.Orders.AnyAsync(o =>
            o.MechanicId == mechanicId &&
            o.StatusId != StatusIds.Completed &&
            o.StatusId != StatusIds.Cancelled);
    }

    public async Task<bool> HasActiveOrdersOnDateAsync(int mechanicId, DateTime date)
    {
        var day = date.Date;
        return await _db.Orders.AnyAsync(o =>
            o.MechanicId == mechanicId &&
            o.DateCreated.Date == day &&
            o.StatusId != StatusIds.Completed &&
            o.StatusId != StatusIds.Cancelled);
    }

    public async Task<bool> IsBusyAtAsync(int mechanicId, DateTime dateTime, int? excludeOrderId = null)
    {
        var day = dateTime.Date;
        return await _db.Orders.AnyAsync(o =>
            o.MechanicId == mechanicId &&
            o.DateCreated.Date == day &&
            o.StatusId != StatusIds.Completed &&
            o.StatusId != StatusIds.Cancelled &&
            (excludeOrderId == null || o.Id != excludeOrderId));
    }
}
