using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string ISBN { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    protected Book() { }

    public Book(string title, string author, string isbn, decimal price, int stockQuantity)
    {
        // Aqui poderíamos ter Value Objects (como um objeto ISBN ou Money), 
        // mas para manter prático, validaremos via Application/Domain Services.
        Title = title;
        Author = author;
        ISBN = isbn;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void UpdateDetails(string title, string author, decimal price)
    {
        Title = title;
        Author = author;
        Price = price;
        UpdateTimestamp();
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        UpdateTimestamp();
    }
}
