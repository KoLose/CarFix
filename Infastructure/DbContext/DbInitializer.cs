using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.DbContext;

public static class DbInitializer
{
    public static async Task InitializeAsync(ContextDb db)
    {
        if (!await db.Database.CanConnectAsync())
            throw new InvalidOperationException(
                "Нет подключения к PostgreSQL. Запустите службу PostgreSQL или Docker (порт 5432/5433).");

        // Если таблицы уже созданы сидом — не трогаем схему
        var created = await db.Database.EnsureCreatedAsync();

        try
        {
            if (await db.Roles.AnyAsync())
                return;
        }
        catch (Exception) when (!created)
        {
            // схема не совпала — пробуем создать недостающее
            await db.Database.EnsureCreatedAsync();
            if (await db.Roles.AnyAsync())
                return;
        }

        await SeedAsync(db);
    }

    private static async Task SeedAsync(ContextDb db)
    {
        var roles = new[]
        {
            new Role { Name = "Admin" },
            new Role { Name = "Mechanic" },
            new Role { Name = "Manager" },
            new Role { Name = "Client" }
        };
        db.Roles.AddRange(roles);
        await db.SaveChangesAsync();

        var statuses = new[]
        {
            new Status { Name = "Новый" },
            new Status { Name = "В работе" },
            new Status { Name = "Завершен" },
            new Status { Name = "Отменен" }
        };
        db.Statuses.AddRange(statuses);
        await db.SaveChangesAsync();

        var adminRole = roles.First(r => r.Name == "Admin").Id;
        var mechRole = roles.First(r => r.Name == "Mechanic").Id;
        var mgrRole = roles.First(r => r.Name == "Manager").Id;
        var clientRole = roles.First(r => r.Name == "Client").Id;

        var admin = new User { Name = "Админ Иванов", Login = "admin", Password = "admin", RoleId = adminRole };
        var mech1 = new User { Name = "Механик Петров", Login = "mech1", Password = "mech1", RoleId = mechRole };
        var mech2 = new User { Name = "Механик Сидоров", Login = "mech2", Password = "mech2", RoleId = mechRole };
        var manager = new User { Name = "Менеджер Козлова", Login = "manager", Password = "manager", RoleId = mgrRole };
        var client1 = new User { Name = "Клиент Орлов", Login = "client1", Password = "client1", RoleId = clientRole };
        var client2 = new User { Name = "Клиент Волкова", Login = "client2", Password = "client2", RoleId = clientRole };
        db.Users.AddRange(admin, mech1, mech2, manager, client1, client2);
        await db.SaveChangesAsync();

        var s1 = new Service { Name = "Замена масла", Description = "Замена моторного масла и фильтра", Price = 2500 };
        var s2 = new Service { Name = "Диагностика", Description = "Компьютерная диагностика", Price = 1500 };
        var s3 = new Service { Name = "Замена колодок", Description = "Замена тормозных колодок", Price = 4000 };
        var s4 = new Service { Name = "Шиномонтаж", Description = "Снятие/установка и балансировка", Price = 2000 };
        db.Services.AddRange(s1, s2, s3, s4);
        await db.SaveChangesAsync();

        db.UserServices.AddRange(
            new UserService { UserId = mech1.Id, ServiceId = s1.Id },
            new UserService { UserId = mech1.Id, ServiceId = s2.Id },
            new UserService { UserId = mech1.Id, ServiceId = s3.Id },
            new UserService { UserId = mech2.Id, ServiceId = s2.Id },
            new UserService { UserId = mech2.Id, ServiceId = s4.Id });

        var sup1 = new Supplier { Name = "АвтоДеталь", Phone = "+7-900-111-22-33" };
        var sup2 = new Supplier { Name = "ЗапЧастьПро", Phone = "+7-900-444-55-66" };
        db.Suppliers.AddRange(sup1, sup2);
        await db.SaveChangesAsync();

        var p1 = new Part { Article = "OIL-5W30", Name = "Масло 5W-30", Quantity = 20, PurchasePrice = 800, SalePrice = 1200, SupplierId = sup1.Id };
        var p2 = new Part { Article = "FLT-001", Name = "Масляный фильтр", Quantity = 15, PurchasePrice = 300, SalePrice = 500, SupplierId = sup1.Id };
        var p3 = new Part { Article = "BRK-FR", Name = "Колодки передние", Quantity = 8, PurchasePrice = 1500, SalePrice = 2500, SupplierId = sup2.Id };
        var p4 = new Part { Article = "TIRE-R", Name = "Ремкомплект шины", Quantity = 2, PurchasePrice = 400, SalePrice = 700, SupplierId = sup2.Id };
        db.Parts.AddRange(p1, p2, p3, p4);
        await db.SaveChangesAsync();

        var car1 = new Car { VIN = "XTA21703000000001", Brand = "Lada", Model = "Vesta", Year = 2020, PlateNumber = "A123BC777", Mileage = 45000, ClientId = client1.Id };
        var car2 = new Car { VIN = "XTA21703000000002", Brand = "Toyota", Model = "Camry", Year = 2018, PlateNumber = "B456DE199", Mileage = 98000, ClientId = client2.Id };
        db.Cars.AddRange(car1, car2);
        await db.SaveChangesAsync();

        var stNew = statuses.First(s => s.Name == "Новый").Id;
        var stWork = statuses.First(s => s.Name == "В работе").Id;
        var stDone = statuses.First(s => s.Name == "Завершен").Id;

        var o1 = new Order { DateCreated = DateTime.UtcNow.AddDays(-3), StatusId = stWork, AutomobileId = car1.Id, MechanicId = mech1.Id };
        var o2 = new Order { DateCreated = DateTime.UtcNow.AddDays(-2), StatusId = stNew, AutomobileId = car2.Id, MechanicId = mech2.Id };
        var o3 = new Order
        {
            DateCreated = DateTime.UtcNow.AddDays(-10),
            DateFinished = DateTime.UtcNow.AddDays(-10).AddHours(6),
            StatusId = stDone,
            AutomobileId = car1.Id,
            MechanicId = mech1.Id
        };
        db.Orders.AddRange(o1, o2, o3);
        await db.SaveChangesAsync();

        db.OrderServices.AddRange(
            new OrderService { OrderId = o1.Id, ServiceId = s1.Id, ActualPrice = 2500, IsCompleted = false },
            new OrderService { OrderId = o1.Id, ServiceId = s2.Id, ActualPrice = 1500, IsCompleted = false },
            new OrderService { OrderId = o2.Id, ServiceId = s4.Id, ActualPrice = 2000, IsCompleted = false },
            new OrderService { OrderId = o3.Id, ServiceId = s3.Id, ActualPrice = 4000, IsCompleted = true });

        db.OrderParts.AddRange(
            new OrderPart { OrderId = o1.Id, PartId = p1.Id, Quantity = 4 },
            new OrderPart { OrderId = o1.Id, PartId = p2.Id, Quantity = 1 },
            new OrderPart { OrderId = o3.Id, PartId = p3.Id, Quantity = 1 });

        db.Payments.AddRange(
            new Payment { OrderId = o3.Id, Amount = 6500, Method = "Карта" },
            new Payment { OrderId = o1.Id, Amount = 2000, Method = "Наличные" });

        db.Comments.AddRange(
            new Comment { OrderId = o1.Id, Text = "Нужна проверка уровня масла", CreatedAt = DateTime.UtcNow.AddDays(-3), IsFinalClientReview = false },
            new Comment { OrderId = o3.Id, Text = "Отличный сервис, спасибо!", CreatedAt = DateTime.UtcNow.AddDays(-10), IsFinalClientReview = true });

        var days = new[] { "Пн", "Вт", "Ср", "Чт", "Пт" };
        foreach (var day in days)
        {
            db.Shifts.Add(new Shift { Name = "Утро", DayOfWeek = day, UserId = day is "Ср" or "Пт" ? mech2.Id : mech1.Id });
            db.Shifts.Add(new Shift { Name = "Вечер", DayOfWeek = day, UserId = day is "Ср" or "Пт" ? mech1.Id : mech2.Id });
        }

        await db.SaveChangesAsync();
    }
}
