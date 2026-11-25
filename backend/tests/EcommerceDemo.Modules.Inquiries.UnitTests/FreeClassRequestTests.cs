using EcommerceDemo.BuildingBlocks.Application;
using EcommerceDemo.Modules.Inquiries.Application.FreeClassRequests;
using EcommerceDemo.Modules.Inquiries.Domain.FreeClassRequests;

using FluentValidation;

namespace EcommerceDemo.Modules.Inquiries.UnitTests;

public class FreeClassRequestTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 11, 25, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_trims_text_and_stores_utc()
    {
        var request = FreeClassRequest.Create(
            Guid.NewGuid(),
            "  Ana Pérez  ",
            " +593 98 420 4476 ",
            " Mestizo ",
            " 2 años ",
            " Tira de la correa ",
            new DateTimeOffset(2026, 11, 25, 10, 0, 0, TimeSpan.FromHours(-5)));

        Assert.Equal("Ana Pérez", request.OwnerName);
        Assert.Equal("+593 98 420 4476", request.Phone);
        Assert.Equal("Mestizo", request.Breed);
        Assert.Equal("2 años", request.DogAge);
        Assert.Equal("Tira de la correa", request.BehaviorProblem);
        Assert.Equal(TimeSpan.Zero, request.CreatedAt.Offset);
        Assert.Equal(15, request.CreatedAt.Hour);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("telefono-no")]
    public void Create_rejects_a_phone_without_seven_to_fifteen_digits(string phone)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            FreeClassRequest.Create(Guid.NewGuid(), "Ana", phone, "Mestizo", "2 años", "Ladra", CreatedAt));

        Assert.Equal("phone", exception.ParamName);
    }

    [Fact]
    public void Create_rejects_an_empty_id()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            FreeClassRequest.Create(Guid.Empty, "Ana", "0984204476", "Mestizo", "2 años", "Ladra", CreatedAt));

        Assert.Equal("id", exception.ParamName);
    }
}

public class SubmitFreeClassRequestValidatorTests
{
    private readonly SubmitFreeClassRequestCommandValidator _validator = new();

    [Fact]
    public void Accepts_a_complete_request()
    {
        var result = _validator.Validate(Sample());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Reports_camel_case_field_names()
    {
        var result = _validator.Validate(Sample() with { OwnerName = " ", Phone = "abc" });

        Assert.Contains(result.Errors, error => error.PropertyName == "ownerName");
        Assert.Contains(result.Errors, error => error.PropertyName == "phone");
    }

    private static SubmitFreeClassRequestCommand Sample() =>
        new("Ana Pérez", "0984204476", "Mestizo", "8 meses", "Ladra cuando se queda solo");
}

public class SubmitFreeClassRequestHandlerTests
{
    [Fact]
    public async Task Does_not_store_an_invalid_request()
    {
        var repository = new RecordingRepository();
        var handler = new SubmitFreeClassRequestHandler(repository, new SubmitFreeClassRequestCommandValidator());

        var outcome = await handler.Handle(
            new SubmitFreeClassRequestCommand("", "1", "", "", ""),
            CancellationToken.None);

        Assert.IsType<SubmitFreeClassRequestResponse.Invalid>(outcome);
        Assert.Equal(0, repository.Added);
    }

    [Fact]
    public async Task Stores_a_valid_request()
    {
        var repository = new RecordingRepository();
        var handler = new SubmitFreeClassRequestHandler(repository, new SubmitFreeClassRequestCommandValidator());
        var command = new SubmitFreeClassRequestCommand("Ana Pérez", "0984204476", "Mestizo", "8 meses", "Ladra en casa");

        var outcome = await handler.Handle(command, CancellationToken.None);

        var created = Assert.IsType<SubmitFreeClassRequestResponse.Created>(outcome);
        Assert.Equal(1, repository.Added);
        Assert.Equal("Ana Pérez", created.Request.OwnerName);
        Assert.Equal("Ladra en casa", created.Request.BehaviorProblem);
        Assert.NotEqual(Guid.Empty, created.Request.Id);
    }

    private sealed class RecordingRepository : IFreeClassRequestRepository
    {
        public int Added { get; private set; }

        public Task AddAsync(FreeClassRequest request, CancellationToken cancellationToken)
        {
            Added++;
            return Task.CompletedTask;
        }

        public Task<PagedResult<FreeClassRequestDto>> ListAsync(int page, int pageSize, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<FreeClassRequestDto>([], page, pageSize, 0));
    }
}

public class ListFreeClassRequestsHandlerTests
{
    [Fact]
    public async Task Does_not_read_when_the_page_is_invalid()
    {
        var repository = new CountingRepository();
        var handler = new ListFreeClassRequestsHandler(repository, new ListFreeClassRequestsQueryValidator());

        var outcome = await handler.Handle(new ListFreeClassRequestsQuery(0, 20), CancellationToken.None);

        Assert.IsType<ListFreeClassRequestsResponse.Invalid>(outcome);
        Assert.Equal(0, repository.Calls);
    }

    private sealed class CountingRepository : IFreeClassRequestRepository
    {
        public int Calls { get; private set; }

        public Task AddAsync(FreeClassRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<PagedResult<FreeClassRequestDto>> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new PagedResult<FreeClassRequestDto>([], page, pageSize, 0));
        }
    }
}
