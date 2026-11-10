using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EcommerceDemo.Modules.Users.Infrastructure.Identity;

public sealed class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=ecommerce;Username=ecommerce;Password=ecommerce",
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", UsersDbContext.Schema))
            .Options;

        return new UsersDbContext(options);
    }
}
