using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.DbContext;

public class ContextDb : Microsoft.EntityFrameworkCore.DbContext
{
    public ContextDb()
    {
    }

    public ContextDb(DbContextOptions<ContextDb> options) : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<UserService> UserServices => Set<UserService>();
    public DbSet<OrderService> OrderServices => Set<OrderService>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<OrderPart> OrderParts => Set<OrderPart>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<PartRequest> PartRequests => Set<PartRequest>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var cs = ConnectionHelper.GetConnectionString();
            if (ConnectionHelper.UseSqlite || cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
                optionsBuilder.UseSqlite(cs);
            else
                optionsBuilder.UseNpgsql(cs);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Login).IsRequired();
            e.Property(x => x.Password).IsRequired();
            e.HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<Car>(e =>
        {
            e.ToTable("Cars");
            e.HasKey(x => x.Id);
            e.Property(x => x.VIN).IsRequired();
            e.Property(x => x.Brand).IsRequired();
            e.Property(x => x.Model).IsRequired();
            e.Property(x => x.PlateNumber).IsRequired();
            e.HasOne(x => x.Client).WithMany(u => u.Cars).HasForeignKey(x => x.ClientId);
        });

        modelBuilder.Entity<Status>(e =>
        {
            e.ToTable("Statuses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Status).WithMany(s => s.Orders).HasForeignKey(x => x.StatusId);
            e.HasOne(x => x.Automobile).WithMany(c => c.Orders).HasForeignKey(x => x.AutomobileId);
            e.HasOne(x => x.Mechanic).WithMany(u => u.Orders).HasForeignKey(x => x.MechanicId);
        });

        modelBuilder.Entity<Service>(e =>
        {
            e.ToTable("Services");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Price).HasColumnType("numeric");
        });

        modelBuilder.Entity<UserService>(e =>
        {
            e.ToTable("UserServices");
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany(u => u.UserServices).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Service).WithMany(s => s.UserServices).HasForeignKey(x => x.ServiceId);
        });

        modelBuilder.Entity<OrderService>(e =>
        {
            e.ToTable("OrderServices");
            e.HasKey(x => x.Id);
            e.Property(x => x.ActualPrice).HasColumnType("numeric");
            e.Property(x => x.IsCompleted).HasDefaultValue(false);
            e.HasOne(x => x.Order).WithMany(o => o.OrderServices).HasForeignKey(x => x.OrderId);
            e.HasOne(x => x.Service).WithMany(s => s.OrderServices).HasForeignKey(x => x.ServiceId);
        });

        modelBuilder.Entity<Supplier>(e =>
        {
            e.ToTable("Suppliers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Phone).IsRequired();
        });

        modelBuilder.Entity<Part>(e =>
        {
            e.ToTable("Parts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Article).IsRequired();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.PurchasePrice).HasColumnType("numeric");
            e.Property(x => x.SalePrice).HasColumnType("numeric");
            e.HasOne(x => x.Supplier).WithMany(s => s.Parts).HasForeignKey(x => x.SupplierId);
        });

        modelBuilder.Entity<OrderPart>(e =>
        {
            e.ToTable("OrderParts");
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Order).WithMany(o => o.OrderParts).HasForeignKey(x => x.OrderId);
            e.HasOne(x => x.Part).WithMany(p => p.OrderParts).HasForeignKey(x => x.PartId);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("Payments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("numeric");
            e.Property(x => x.Method).IsRequired();
            e.HasOne(x => x.Order).WithMany(o => o.Payments).HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.ToTable("Comments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Text).IsRequired();
            e.Property(x => x.IsFinalClientReview).HasDefaultValue(false);
            e.HasOne(x => x.Order).WithMany(o => o.Comments).HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<Shift>(e =>
        {
            e.ToTable("Shifts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.DayOfWeek).IsRequired();
            e.HasOne(x => x.User).WithMany(u => u.Shifts).HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PartRequest>(e =>
        {
            e.ToTable("PartRequests");
            e.HasKey(x => x.Id);
            e.Property(x => x.ItemName).IsRequired();
            e.HasOne(x => x.User).WithMany(u => u.PartRequests).HasForeignKey(x => x.UserId);
        });
    }
}
