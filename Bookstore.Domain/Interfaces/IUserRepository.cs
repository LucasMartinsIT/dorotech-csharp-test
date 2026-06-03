using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Entities;

namespace Bookstore.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
}
