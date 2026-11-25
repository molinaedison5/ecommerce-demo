using EcommerceDemo.Modules.Inquiries.Domain.FreeClassRequests;

using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Modules.Inquiries.Infrastructure.Persistence;

public sealed class InquiriesDbContext(DbContextOptions<InquiriesDbContext> options) : DbContext(options)
{
    public const string Schema = "inquiries";

    public DbSet<FreeClassRequest> FreeClassRequests => Set<FreeClassRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InquiriesDbContext).Assembly);
    }
}
