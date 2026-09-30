using LoginApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using LoginApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o OpenAPI e configura o JWT para aparecer no Swagger.
builder.Services.AddOpenApi(options =>
{
options.AddDocumentTransformer((document, context, cancellationToken) =>
{
document.Components ??= new OpenApiComponents();

    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        In = ParameterLocation.Header,
        BearerFormat = "JWT"
    };

    return Task.CompletedTask;
});

options.AddOperationTransformer((operation, context, cancellationToken) =>
{
    var protegido = context.Description.ActionDescriptor.EndpointMetadata
        .OfType<AuthorizeAttribute>()
        .Any();

    // Mostra no Swagger quais endpoints precisam de autenticação.
    if (protegido)
    {
        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            }] = Array.Empty<string>()
        });
    }

    return Task.CompletedTask;
});

});

builder.Services.AddControllers();
builder.Services.AddScoped<UsuarioService>();

// Conexão com o banco usado pela aplicação.
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")
));

// Configuração da autenticação com JWT.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!
            )
        )
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// O Swagger fica disponível durante o desenvolvimento.
if (app.Environment.IsDevelopment())
{
app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "LoginApi");
});

}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}