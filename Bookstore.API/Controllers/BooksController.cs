using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Retorna a lista de livros cadastrados.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? title = null, [FromQuery] string? author = null)
    {
        var (books, totalCount) = await _bookService.GetAllPagedAsync(page, pageSize, title, author);

        // Mandando o count no header pra facilitar a vida do front na paginação
        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        return Ok(books);
    }

    /// <summary>
    /// Busca dados de um livro específico.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return Ok(book);
    }

    /// <summary>
    /// Adiciona um livro novo no estoque.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var book = await _bookService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    /// <summary>
    /// Atualiza os dados principais do livro.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto)
    {
        await _bookService.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Deleta o livro (TODO: avaliar soft delete no futuro).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _bookService.DeleteAsync(id);
        return NoContent();
    }
}