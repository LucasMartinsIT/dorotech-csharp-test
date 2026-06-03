using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Gera o token JWT para o usuário logado.
    /// </summary>
    /// <remarks>
    /// Teste local: admin / admin123
    /// </remarks>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    // TODO: Remover essa rota depois, criada só pra quebrar um galho com o hash do admin no banco local
    [HttpPost("register-temp")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterTemp([FromBody] LoginDto dto, [FromServices] Bookstore.Domain.Interfaces.IUserRepository userRepository)
    {
        // BCrypt já cuida da geração do salt por baixo dos panos
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new Bookstore.Domain.Entities.User(dto.Username, hash, Bookstore.Domain.Enums.UserRole.Admin);

        await userRepository.AddAsync(user);

        return Ok(new { Message = "Usuário salvo", HashGerado = hash });
    }
}