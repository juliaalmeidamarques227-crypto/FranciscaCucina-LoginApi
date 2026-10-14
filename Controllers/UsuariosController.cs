using LoginApi.DTOs;
using LoginApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Cadastrar(CadastroUsuarioDto cadastro)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.Cadastrar(cadastro);

        if (!resultado.sucesso)
        {
            return Conflict(resultado.erro);
        }

        return Ok(new
        {
            mensagem = "Usuário cadastrado com sucesso!",
            resultado.usuario!.Id,
            resultado.usuario.Nome,
            resultado.usuario.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUsuarioDto login)
    {
        var resultado = await _usuarioService.Login(login);

        if (resultado.usuario == null)
        {
            if (resultado.erro == "Usuário não encontrado.")
            {
                return NotFound(resultado.erro);
            }

            return Unauthorized(resultado.erro);
        }

        var token = _usuarioService.GerarToken(resultado.usuario);

        return Ok(new
        {
            mensagem = "Login realizado com sucesso!",
            token
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