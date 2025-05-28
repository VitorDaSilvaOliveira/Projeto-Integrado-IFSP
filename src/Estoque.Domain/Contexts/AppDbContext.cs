using Estoque.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Domain.Contexts;

public class AppDbContext(DbContextOptions options) 
    : IdentityDbContext<Usuario>(options);

