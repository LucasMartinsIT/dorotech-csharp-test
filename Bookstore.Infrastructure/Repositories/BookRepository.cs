using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly BookstoreDbContext _context;

    public BookRepository(BookstoreDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Book> Books, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string? titleFilter, string? authorFilter)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleFilter))
            query = query.Where(b => b.Title.ToLower().Contains(titleFilter.ToLower()));

        if (!string.IsNullOrWhiteSpace(authorFilter))
            query = query.Where(b => b.Author.ToLower().Contains(authorFilter.ToLower()));

        // Regra de Negócio: Ordem alfabética ascendente por Título
        query = query.OrderBy(b => b.Title);

        var totalCount = await query.CountAsync();

        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking() // Otimização de performance para consultas de leitura
            .ToListAsync();

        return (books, totalCount);
    }

    public async Task<Book?> GetByIdAsync(Guid id) => await _context.Books.FindAsync(id);

    public async Task<bool> ExistsByIsbnAsync(string isbn) =>
        await _context.Books.AnyAsync(b => b.ISBN == isbn);

    public async Task<bool> ExistsByTitleAndAuthorAsync(string title, string author) =>
        await _context.Books.AnyAsync(b => b.Title.ToLower() == title.ToLower() && b.Author.ToLower() == author.ToLower());

    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}
