using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginApi.Services;

public class UsuarioService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UsuarioService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<(bool sucesso, string? erro, Usuario? usuario)> Cadastrar(
        CadastroUsuarioDto cadastro)
    {
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == cadastro.Email);

        if (emailExiste)
        {
            return (false, "E-mail já cadastrado.", null);
        }

        var usuario = new Usuario
        {
            Nome = cadastro.Nome,
            Email = cadastro.Email
        };

        // A senha fica armazenada apenas como hash.
        usuario.SenhaHash = _passwordHasher.HashPassword(
            usuario,
            cadastro.Senha
        );

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return (true, null, usuario);
    }

    public async Task<(Usuario? usuario, string? erro)> Login(
        LoginUsuarioDto login)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == login.Email);

        if (usuario == null)
        {
            return (null, "Usuário não encontrado.");
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            login.Senha
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return (null, "Senha incorreta.");
        }

        return (usuario, null);
    }

    public string GerarToken(Usuario usuario)
    {
        // Essas informações ficam dentro do token para identificar o usuário.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        var credenciais = new SigningCredentials(
            chave,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}