using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class OrderRepository
{
    private readonly ContextDb _db;

    public OrderRepository(ContextDb db) => _db = db;

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders
            .Include(o => o.Status)
            .Include(o => o.Automobile)!.ThenInclude(c => c!.Client)
            .Include(o => o.Mechanic)
            .Include(o => o.OrderServices)!.ThenInclude(os => os.Service)
            .Include(o => o.OrderParts)!.ThenInclude(op => op.Part)
            .Include(o => o.Comments)
            .OrderByDescending(o => o.DateCreated)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByMechanicAsync(int mechanicId)
    {
        return await _db.Orders
            .Include(o => o.Status)
            .Include(o => o.Automobile)!.ThenInclude(c => c!.Client)
            .Include(o => o.Mechanic)
            .Include(o => o.OrderServices)!.ThenInclude(os => os.Service)
            .Include(o => o.OrderParts)!.ThenInclude(op => op.Part)
            .Include(o => o.Comments)
            .Where(o => o.MechanicId == mechanicId)
            .OrderByDescending(o => o.DateCreated)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders
            .Include(o => o.Status)
            .Include(o => o.Automobile)!.ThenInclude(c => c!.Client)
            .Include(o => o.Mechanic)
            .Include(o => o.OrderServices)!.ThenInclude(os => os.Service)
            .Include(o => o.OrderParts)!.ThenInclude(op => op.Part)
            .Include(o => o.Comments)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
    }

    public async Task AddAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
    }

    public async Task CompleteServiceAsync(OrderService orderService)
    {
        orderService.IsCompleted = true;
        _db.OrderServices.Update(orderService);
        await _db.SaveChangesAsync();
    }
}
