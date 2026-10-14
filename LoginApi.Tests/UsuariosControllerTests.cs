using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LoginApi.Tests;

public class UsuariosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsuariosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = CriarCliente();
    }

    private HttpClient CriarCliente(string? perfilTeste = null)
    {
        var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            }
        );

        if (!string.IsNullOrWhiteSpace(perfilTeste))
        {
            client.DefaultRequestHeaders.Add(
                "X-Test-Role",
                perfilTeste
            );
        }

        return client;
    }

    [Fact]
    public async Task CadastrarUsuarioValido_DeveRetornar200()
    {
        using var adminClient = CriarCliente("Admin");

        var cadastro = new
        {
            nome = "Teste Automatizado",
            email = $"teste{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var response = await adminClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CadastrarEmailDuplicado_DeveRetornar409()
    {
        using var adminClient = CriarCliente("Admin");

        var email = $"duplicado{Guid.NewGuid()}@teste.com";

        var cadastro = new
        {
            nome = "Usuario Teste",
            email = email,
            senha = "123456"
        };

        var primeiroCadastro = await adminClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(
            HttpStatusCode.OK,
            primeiroCadastro.StatusCode
        );

        var segundoCadastro = await adminClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(
            HttpStatusCode.Conflict,
            segundoCadastro.StatusCode
        );
    }

    [Fact]
    public async Task LoginComCredenciaisValidas_DeveRetornar200EToken()
    {
        using var adminClient = CriarCliente("Admin");

        var cadastro = new
        {
            nome = "Teste Login",
            email = $"login{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var cadastroResponse = await adminClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(
            HttpStatusCode.OK,
            cadastroResponse.StatusCode
        );

        var login = new
        {
            email = cadastro.email,
            senha = cadastro.senha
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios/login",
            login
        );

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        var conteudo =
            await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.True(
            conteudo.TryGetProperty("token", out var token)
        );

        Assert.False(
            string.IsNullOrWhiteSpace(token.GetString())
        );
    }

    [Fact]
    public async Task LoginComSenhaIncorreta_DeveRetornar401()
    {
        using var adminClient = CriarCliente("Admin");

        var cadastro = new
        {
            nome = "Teste Senha",
            email = $"senha{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var cadastroResponse = await adminClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(
            HttpStatusCode.OK,
            cadastroResponse.StatusCode
        );

        var login = new
        {
            email = cadastro.email,
            senha = "senhaerrada"
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios/login",
            login
        );

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            loginResponse.StatusCode
        );
    }

    [Fact]
    public async Task AcessarRotaProtegidaSemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync(
            "/api/Usuarios/protegido"
        );

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [Fact]
    public async Task UsuarioComumAcessandoAdmin_DeveRetornar403()
    {
        using var adminClient = CriarCliente("Admin");

        var cadastro = new
        {
            nome = "Teste Usuario",
            email = $"usuario{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var cadastroResponse = await adminClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(
            HttpStatusCode.OK,
            cadastroResponse.StatusCode
        );

        var login = new
        {
            email = cadastro.email,
            senha = cadastro.senha
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Usuarios/login",
            login
        );

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        var conteudo =
            await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = conteudo
            .GetProperty("token")
            .GetString();

        Assert.False(
            string.IsNullOrWhiteSpace(token)
        );

        using var usuarioClient = CriarCliente();

        usuarioClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token
            );

        var adminResponse = await usuarioClient.GetAsync(
            "/api/Usuarios/admin"
        );

        Assert.Equal(
            HttpStatusCode.Forbidden,
            adminResponse.StatusCode
        );
    }
        [Fact]
    public async Task UsuarioComumTentandoCadastrar_DeveRetornar403()
    {
        using var usuarioClient = CriarCliente("Usuario");

        var cadastro = new
        {
            nome = "Cadastro Não Permitido",
            email = $"naopermitido{Guid.NewGuid()}@teste.com",
            senha = "123456"
        };

        var response = await usuarioClient.PostAsJsonAsync(
            "/api/Usuarios",
            cadastro
        );

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode
        );
    }
}