using EcommerceDemo.Modules.Inquiries.Domain.FreeClassRequests;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDemo.Modules.Inquiries.Infrastructure.Persistence;

internal sealed class FreeClassRequestConfiguration : IEntityTypeConfiguration<FreeClassRequest>
{
    public void Configure(EntityTypeBuilder<FreeClassRequest> builder)
    {
        builder.ToTable("free_class_requests");
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Id).HasColumnName("id");
        builder.Property(request => request.OwnerName).HasColumnName("owner_name").HasMaxLength(FreeClassRequest.OwnerNameMaxLength).IsRequired();
        builder.Property(request => request.Phone).HasColumnName("phone").HasMaxLength(FreeClassRequest.PhoneMaxLength).IsRequired();
        builder.Property(request => request.Breed).HasColumnName("breed").HasMaxLength(FreeClassRequest.BreedMaxLength).IsRequired();
        builder.Property(request => request.DogAge).HasColumnName("dog_age").HasMaxLength(FreeClassRequest.DogAgeMaxLength).IsRequired();
        builder.Property(request => request.BehaviorProblem).HasColumnName("behavior_problem").HasMaxLength(FreeClassRequest.BehaviorProblemMaxLength).IsRequired();
        builder.Property(request => request.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.HasIndex(request => request.CreatedAt);
    }
}
