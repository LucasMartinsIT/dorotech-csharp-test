using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Application.DTOs;
using Bookstore.Application.Services;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using Moq;
using Xunit;

namespace Bookstore.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        // Configuramos o Mock (dublê de testes) do nosso repositório
        _bookRepositoryMock = new Mock<IBookRepository>();

        // Injetamos o mock no serviço
        _bookService = new BookService(_bookRepositoryMock.Object);
    }

    [Fact(DisplayName = "CreateAsync - Deve lançar exceção quando ISBN já existir")]
    public async Task CreateAsync_ShouldThrowException_WhenIsbnAlreadyExists()
    {
        // Arrange (Preparação)
        var dto = new CreateBookDto("Novo Livro", "Autor Teste", "123456789", 50.0m, 10);

        // Simulamos que o banco vai responder "true" para a verificação de ISBN
        _bookRepositoryMock.Setup(repo => repo.ExistsByIsbnAsync(dto.ISBN))
                           .ReturnsAsync(true);

        // Act & Assert (Ação e Verificação)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookService.CreateAsync(dto));

        Assert.Equal("Já existe um livro cadastrado com este ISBN.", exception.Message);

        // Garante que o método AddAsync NUNCA foi chamado
        _bookRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact(DisplayName = "CreateAsync - Deve criar livro com sucesso quando dados forem válidos")]
    public async Task CreateAsync_ShouldCreateBook_WhenDataIsValid()
    {
        // Arrange
        var dto = new CreateBookDto("Clean Architecture", "Robert C. Martin", "987654321", 120.0m, 5);

        // Simulamos que o livro NÃO existe no banco
        _bookRepositoryMock.Setup(repo => repo.ExistsByIsbnAsync(dto.ISBN)).ReturnsAsync(false);
        _bookRepositoryMock.Setup(repo => repo.ExistsByTitleAndAuthorAsync(dto.Title, dto.Author)).ReturnsAsync(false);

        // Act
        var result = await _bookService.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);

        // Garante que o método AddAsync FOI chamado exatamente UMA vez
        _bookRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Once);
    }
}