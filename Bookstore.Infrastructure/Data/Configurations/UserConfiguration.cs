using System;
using System.Collections.Generic;
using System.Text;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        // Data Seeding: Criando o usuário Admin inicial
        // Nota: A senha em produção jamais deve ser exposta. 
        // Aqui estamos usando um hash BCrypt pré-gerado para a senha "admin123"
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var adminUser = new User("admin", "$2a$11$jI2H.Xq2jVlU4/BfV.QyQeX1B6V8/z8U.C8nQvUoKz1G/T7ZqX5mS", UserRole.Admin);

        // Usando reflexão ou manipulação de estado do EF para injetar o ID no Seed, 
        // já que o `set` do Id é protected no BaseEntity.
        adminUser.GetType().GetProperty("Id")?.SetValue(adminUser, adminId);

        builder.HasData(adminUser);
    }
}
