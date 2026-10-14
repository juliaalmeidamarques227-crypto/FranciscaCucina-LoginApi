using LoginApi.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

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

            // Mantém o JWT real como autenticação padrão.
            // O esquema de teste só é usado quando o teste envia
            // o header X-Test-Role.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestOrJwt";
                options.DefaultChallengeScheme = "TestOrJwt";
            })
            .AddPolicyScheme("TestOrJwt", null, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.ContainsKey("X-Test-Role"))
                    {
                        return "Test";
                    }

                    return JwtBearerDefaults.AuthenticationScheme;
                };
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                "Test",
                _ => { }
            );

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

internal class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Role", out var role))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            return Task.FromResult(AuthenticateResult.Fail("Perfil de teste não informado."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user"),
            new Claim(ClaimTypes.Name, "Usuário de Teste"),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            Scheme.Name
        );

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            Scheme.Name
        );

        return Task.FromResult(
            AuthenticateResult.Success(ticket)
        );
    }
}