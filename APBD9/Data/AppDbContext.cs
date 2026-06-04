namespace APBD9.Data;
using Microsoft.EntityFrameworkCore;
using APBD9.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<UserNote> UserNotes => Set<UserNote>();
}