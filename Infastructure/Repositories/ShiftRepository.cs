using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class ShiftRepository
{
    private readonly ContextDb _db;

    public ShiftRepository(ContextDb db) => _db = db;

    public async Task<List<Shift>> GetAllAsync()
    {
        return await _db.Shifts
            .Include(s => s.User)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task UpdateAsync(Shift shift)
    {
        _db.Shifts.Update(shift);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> HasFreeShiftAsync(int userId, DateTime date)
    {
        var dayName = GetDayName(date.DayOfWeek);
        return await _db.Shifts.AnyAsync(s => s.UserId == userId && s.DayOfWeek == dayName);
    }

    public async Task<List<Shift>> GetByUserAsync(int userId)
    {
        return await _db.Shifts.Where(s => s.UserId == userId).ToListAsync();
    }

    public static string GetDayName(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "Пн",
        DayOfWeek.Tuesday => "Вт",
        DayOfWeek.Wednesday => "Ср",
        DayOfWeek.Thursday => "Чт",
        DayOfWeek.Friday => "Пт",
        DayOfWeek.Saturday => "Сб",
        DayOfWeek.Sunday => "Вс",
        _ => day.ToString()
    };
}
