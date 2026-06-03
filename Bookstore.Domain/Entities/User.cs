using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Enums;

namespace Bookstore.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    // Construtor para o EF e inicialização
    protected User() { }

    public User(string username, string passwordHash, UserRole role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}
