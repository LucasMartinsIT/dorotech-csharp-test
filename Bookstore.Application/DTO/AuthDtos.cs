using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.DTOs;

public record LoginDto(string Username, string Password);

public record TokenResponseDto(string Token, int ExpirationInMinutes);
