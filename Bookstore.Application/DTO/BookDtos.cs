using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.DTOs;

public record BookResponseDto(Guid Id, string Title, string Author, string ISBN, decimal Price, int StockQuantity);

public record CreateBookDto(string Title, string Author, string ISBN, decimal Price, int StockQuantity);

public record UpdateBookDto(string Title, string Author, decimal Price);