using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;

namespace Bookstore.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<(IEnumerable<BookResponseDto> Books, int TotalCount)> GetAllPagedAsync(int page, int pageSize, string? title, string? author)
    {
        var (books, totalCount) = await _bookRepository.GetAllPagedAsync(page, pageSize, title, author);

        // Fazendo o mapeamento manual pra DTO (TODO: avaliar colocar o AutoMapper se as entidades crescerem muito)
        var dtos = books.Select(b => new BookResponseDto(b.Id, b.Title, b.Author, b.ISBN, b.Price, b.StockQuantity));

        return (dtos, totalCount);
    }

    public async Task<BookResponseDto> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Livro não encontrado."); // Lança 404 direto no middleware

        return new BookResponseDto(book.Id, book.Title, book.Author, book.ISBN, book.Price, book.StockQuantity);
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
    {
        // Barrando duplicidade logo de cara pra evitar stress no banco
        if (await _bookRepository.ExistsByIsbnAsync(dto.ISBN))
            throw new InvalidOperationException("Já existe um livro cadastrado com este ISBN.");

        if (await _bookRepository.ExistsByTitleAndAuthorAsync(dto.Title, dto.Author))
            throw new InvalidOperationException("Este livro já está cadastrado para este autor.");

        var book = new Book(dto.Title, dto.Author, dto.ISBN, dto.Price, dto.StockQuantity);

        await _bookRepository.AddAsync(book);

        return new BookResponseDto(book.Id, book.Title, book.Author, book.ISBN, book.Price, book.StockQuantity);
    }

    public async Task UpdateAsync(Guid id, UpdateBookDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Livro não encontrado.");

        book.UpdateDetails(dto.Title, dto.Author, dto.Price);
        await _bookRepository.UpdateAsync(book);
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Livro não encontrado.");

        await _bookRepository.DeleteAsync(book);
    }
}