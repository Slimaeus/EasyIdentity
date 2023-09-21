using EasyIdentity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyIdentity.Core.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
}
