using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(150);
        builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
        builder.Property(b => b.Price).HasPrecision(18, 2);

        // Índice para evitar duplicação em nível de banco (Performance e Consistência)
        builder.HasIndex(b => b.ISBN).IsUnique();
        builder.HasIndex(b => new { b.Title, b.Author }).IsUnique();

        // Data Seeding: Massa de dados inicial para testes
        var book1 = new Book("Clean Code", "Robert C. Martin", "9780132350884", 150.00m, 10);
        book1.GetType().GetProperty("Id")?.SetValue(book1, Guid.Parse("22222222-2222-2222-2222-222222222222"));

        var book2 = new Book("Domain-Driven Design", "Eric Evans", "9780321125217", 200.00m, 5);
        book2.GetType().GetProperty("Id")?.SetValue(book2, Guid.Parse("33333333-3333-3333-3333-333333333333"));

        builder.HasData(book1, book2);
    }
}