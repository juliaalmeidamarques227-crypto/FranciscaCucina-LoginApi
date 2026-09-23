using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UsuariosController(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(CadastroUsuario cadastro)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == cadastro.Email);

        if (emailExiste)
        {
            return Conflict("E-mail já cadastrado.");
        }

        var usuario = new Usuario
        {
            Nome = cadastro.Nome,
            Email = cadastro.Email
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(
            usuario,
            cadastro.Senha
        );

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Usuário cadastrado com sucesso!",
            usuario.Id,
            usuario.Nome,
            usuario.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUsuario login)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == login.Email);

        if (usuario == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            login.Senha
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Senha incorreta.");
        }

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

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new
        {
            mensagem = "Login realizado com sucesso!",
            token = tokenString
        });
    }
    [Authorize]
[HttpGet("protegido")]
public IActionResult RotaProtegida()
{
    return Ok("Você está autenticado!");
}
[Authorize(Roles = "Admin")]
[HttpGet("admin")]
public IActionResult AreaAdmin()
{
    return Ok("Você é um administrador!");
}
}