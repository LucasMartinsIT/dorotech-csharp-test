using System;
using System.Collections.Generic;
using Bookstore.Application.DTOs;

namespace Bookstore.Application.Interfaces;

public interface IBookService
{
    Task<(IEnumerable<BookResponseDto> Books, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string? title, string? author);
    Task<BookResponseDto> GetByIdAsync(Guid id);
    Task<BookResponseDto> CreateAsync(CreateBookDto dto);
    Task UpdateAsync(Guid id, UpdateBookDto dto);
    Task DeleteAsync(Guid id);
}

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
}

// A implementação disso aqui tem que ficar na Infra, pra não sujar a Application com lib de JWT
public interface ITokenService
{
    string GenerateToken(string username, string role);
}