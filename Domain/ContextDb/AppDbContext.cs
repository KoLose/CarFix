using Microsoft.EntityFrameworkCore;

namespace Domain.ContextDb;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}