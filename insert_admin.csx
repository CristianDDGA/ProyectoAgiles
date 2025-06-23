#r "nuget: Microsoft.EntityFrameworkCore.SqlServer, 9.0.0"
#r "nuget: BCrypt.Net-Next, 4.0.2"
using System;
using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Infrastructure.Data;

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ProyectoAgilesDB;Trusted_Connection=true;MultipleActiveResultSets=true")
    .Options;

using var context = new ApplicationDbContext(options);

var email = "admin@sistema.com";
var password = "Admin123!";
var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

if (!context.Users.AnyAsync(u => u.Email == email).Result)
{
    var admin = new User
    {
        FirstName = "Admin",
        LastName = "Sistema",
        Email = email,
        PasswordHash = passwordHash,
        UserType = ProyectoAgiles.Domain.Enums.UserType.Admin,
        Cedula = "0000000000",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
    context.Users.Add(admin);
    context.SaveChanges();
    Console.WriteLine("Usuario admin insertado correctamente.");
}
else
{
    Console.WriteLine("Ya existe un usuario admin con ese email.");
}
