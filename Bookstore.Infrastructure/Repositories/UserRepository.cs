using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookstoreDbContext _context;

    public UserRepository(BookstoreDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}