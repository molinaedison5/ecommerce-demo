using System.Reflection;

using EcommerceDemo.Modules.Bookings.Application;
using EcommerceDemo.Modules.Bookings.Domain;
using EcommerceDemo.Modules.Cart.Application;
using EcommerceDemo.Modules.Cart.Domain;
using EcommerceDemo.Modules.Catalog.Application.Products;
using EcommerceDemo.Modules.Catalog.Domain.Products;
using EcommerceDemo.Modules.Orders.Application;
using EcommerceDemo.Modules.Orders.Domain;
using EcommerceDemo.Modules.Payments.Application;
using EcommerceDemo.Modules.Payments.Domain;
using EcommerceDemo.Modules.Users.Application.Auth;
using EcommerceDemo.Modules.Users.Domain;

using NetArchTest.Rules;

namespace EcommerceDemo.Architecture.Tests;

public class ModuleBoundaryTests
{
    private static readonly string[] ModuleRoots =
    [
        "EcommerceDemo.Modules.Bookings",
        "EcommerceDemo.Modules.Cart",
        "EcommerceDemo.Modules.Catalog",
        "EcommerceDemo.Modules.Orders",
        "EcommerceDemo.Modules.Payments",
        "EcommerceDemo.Modules.Users",
    ];

    private static readonly Assembly[] DomainAssemblies =
    [
        typeof(ServiceOffering).Assembly,
        typeof(Cart).Assembly,
        typeof(Product).Assembly,
        typeof(Order).Assembly,
        typeof(PaymentStatus).Assembly,
        typeof(UserProfile).Assembly,
    ];

    private static readonly Assembly[] ApplicationAssemblies =
    [
        typeof(BookingsApplication).Assembly,
        typeof(CartApplication).Assembly,
        typeof(ListProductsHandler).Assembly,
        typeof(OrdersApplication).Assembly,
        typeof(IPaymentGateway).Assembly,
        typeof(IAuthService).Assembly,
    ];

    private static readonly Assembly[] ModuleAssemblies =
    [
        ..DomainAssemblies,
        ..ApplicationAssemblies,
        typeof(EcommerceDemo.Modules.Bookings.BookingsModule).Assembly,
        typeof(EcommerceDemo.Modules.Cart.CartModule).Assembly,
        typeof(EcommerceDemo.Modules.Catalog.CatalogModule).Assembly,
        typeof(EcommerceDemo.Modules.Orders.OrdersModule).Assembly,
        typeof(EcommerceDemo.Modules.Payments.PaymentsModule).Assembly,
        typeof(EcommerceDemo.Modules.Users.UsersModule).Assembly,
    ];

    [Fact]
    public void Modules_do_not_reference_each_other()
    {
        foreach (var assembly in ModuleAssemblies)
        {
            var name = assembly.GetName().Name ?? string.Empty;
            var ownRoot = ModuleRoots.Single(root => name.StartsWith(root, StringComparison.Ordinal));
            var forbidden = ModuleRoots.Where(root => root != ownRoot).ToArray();
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbidden)
                .GetResult();

            Assert.True(result.IsSuccessful, $"{name} depends on another module: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }

    [Fact]
    public void Domain_and_application_do_not_depend_on_infrastructure_concerns()
    {
        foreach (var assembly in DomainAssemblies.Concat(ApplicationAssemblies))
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    "Microsoft.EntityFrameworkCore",
                    "Microsoft.AspNetCore",
                    "Npgsql",
                    "StackExchange")
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"{assembly.GetName().Name} leaked an infrastructure dependency: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }
}
