using Bookstore.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Application.DTOs;
using Bookstore.Domain.Interfaces;
using BCrypt.Net;

namespace Bookstore.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);

        // Bate a senha limpa com o hash salvo no banco. 
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            // O middleware global intercepta isso e cospe um 401 pro client
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
        }

        var token = _tokenService.GenerateToken(user.Username, user.Role.ToString());

        // TODO: A expiração tá chumbada em 2 horas (120 min). Melhor jogar pro appsettings depois.
        return new TokenResponseDto(token, 120);
    }
}