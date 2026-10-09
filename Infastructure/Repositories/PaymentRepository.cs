using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class PaymentRepository
{
    private readonly ContextDb _db;

    public PaymentRepository(ContextDb db) => _db = db;

    public async Task<decimal> GetTotalAsync()
    {
        return await _db.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0m;
    }
}
