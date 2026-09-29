using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LoginApi.Tests;

public class UsuariosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsuariosControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task CadastrarUsuarioValido_DeveRetornar200()
    {
        var cadastro = new
        {
            nome = "Teste Automatizado",
            email = $"teste{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CadastrarEmailDuplicado_DeveRetornar409()
    {
        var email = $"duplicado{Guid.NewGuid()}@teste.com";

        var cadastro = new
        {
            nome = "Usuario Teste",
            email = email,
            senha = "123456"
        };

        var primeiroCadastro = await _client.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.OK, primeiroCadastro.StatusCode);

        var segundoCadastro = await _client.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.Conflict, segundoCadastro.StatusCode);
    }

    [Fact]
    public async Task LoginComCredenciaisValidas_DeveRetornar200EToken()
    {
        var cadastro = new
        {
            nome = "Teste Login",
            email = $"login{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var cadastroResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.OK, cadastroResponse.StatusCode);

        var login = new
        {
            email = cadastro.email,
            senha = cadastro.senha
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios/login",
            login
        );

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var conteudo = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.True(conteudo.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrWhiteSpace(token.GetString()));
    }

    [Fact]
    public async Task LoginComSenhaIncorreta_DeveRetornar401()
    {
        var cadastro = new
        {
            nome = "Teste Senha",
            email = $"senha{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var cadastroResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.OK, cadastroResponse.StatusCode);

        var login = new
        {
            email = cadastro.email,
            senha = "senhaerrada"
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios/login",
            login
        );

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task AcessarRotaProtegidaSemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync(
            "/api/Usuarios/protegido"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UsuarioComumAcessandoAdmin_DeveRetornar403()
    {
        var cadastro = new
        {
            nome = "Teste Usuario",
            email = $"usuario{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var cadastroResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.OK, cadastroResponse.StatusCode);

        var login = new
        {
            email = cadastro.email,
            senha = cadastro.senha
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios/login",
            login
        );

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var conteudo = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = conteudo.GetProperty("token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var adminResponse = await _client.GetAsync(
            "/api/Usuarios/admin"
        );

        Assert.Equal(HttpStatusCode.Forbidden, adminResponse.StatusCode);
    }
}