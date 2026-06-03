using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Entities;

namespace Bookstore.Domain.Interfaces;

public interface IBookRepository
{
    // Retorna uma tupla com os livros e o total de registros para paginação
    Task<(IEnumerable<Book> Books, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string? titleFilter, string? authorFilter);
    Task<Book?> GetByIdAsync(Guid id);
    Task<bool> ExistsByIsbnAsync(string isbn);
    Task<bool> ExistsByTitleAndAuthorAsync(string title, string author);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
}