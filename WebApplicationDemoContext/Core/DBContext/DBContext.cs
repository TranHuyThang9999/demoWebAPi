using Microsoft.EntityFrameworkCore;
using WebApplicationDemoContext.core.Model;

namespace WebApplicationDemoContext.DBContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u =>u.Name).IsUnique();
        }
    }
}