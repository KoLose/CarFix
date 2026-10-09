using Domain.Models;
using Infastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories;

public class CommentRepository
{
    private readonly ContextDb _db;

    public CommentRepository(ContextDb db) => _db = db;

    public async Task AddAsync(Comment comment)
    {
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> HasFinalClientReviewAsync(int orderId)
    {
        return await _db.Comments.AnyAsync(c => c.OrderId == orderId && c.IsFinalClientReview);
    }

    public async Task<List<Comment>> GetByOrderAsync(int orderId)
    {
        return await _db.Comments
            .Where(c => c.OrderId == orderId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}
