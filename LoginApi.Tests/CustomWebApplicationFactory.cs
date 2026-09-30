using LoginApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoginApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
private readonly SqliteConnection _connection;

public CustomWebApplicationFactory()
{
    // O banco fica só durante os testes e não interfere no banco real.
    _connection = new SqliteConnection("DataSource=:memory:");
    _connection.Open();
}

protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.UseEnvironment("Development");

    builder.ConfigureAppConfiguration((_, config) =>
    {
        var settings = new Dictionary<string, string?>
        {
            // Usamos uma configuração própria para os testes.
            ["Jwt:Key"] = "Teste-JWT-Chave-Segura-2026-123456789",
            ["Jwt:Issuer"] = "LoginApi.Tests",
            ["Jwt:Audience"] = "LoginApi.Tests"
        };

        config.AddInMemoryCollection(settings);
    });

    builder.ConfigureServices(services =>
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                typeof(IDbContextOptionsConfiguration<AppDbContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(_connection);
        });

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    });
}

protected override void Dispose(bool disposing)
{
    base.Dispose(disposing);

    if (disposing)
    {
        _connection.Dispose();
    }
}

}