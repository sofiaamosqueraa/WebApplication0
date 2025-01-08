using Microsoft.EntityFrameworkCore;
using WebApplication0.Models; 

namespace WebApplication0.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
