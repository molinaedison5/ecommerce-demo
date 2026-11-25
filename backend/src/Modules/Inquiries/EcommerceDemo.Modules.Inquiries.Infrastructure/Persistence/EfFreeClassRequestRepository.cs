using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;
using EcommerceDemo.Modules.Inquiries.Domain.FreeClassRequests;

using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Modules.Inquiries.Infrastructure.Persistence;

public sealed class EfFreeClassRequestRepository(InquiriesDbContext db) : IFreeClassRequestRepository
{
    public async Task AddAsync(FreeClassRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        db.FreeClassRequests.Add(request);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<FreeClassRequestDto>> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = db.FreeClassRequests.AsNoTracking().OrderByDescending(request => request.CreatedAt);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(request => new FreeClassRequestDto(
                request.Id,
                request.OwnerName,
                request.Phone,
                request.Breed,
                request.DogAge,
                request.BehaviorProblem,
                request.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<FreeClassRequestDto>(items, page, pageSize, total);
    }
}
