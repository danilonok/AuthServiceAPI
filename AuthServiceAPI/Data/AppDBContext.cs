
using AuthServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServiceAPI.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserCredential>()
              //  .HasNoKey();
            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Credential);
        }

        public DbSet<User> Users { get; set; }
    }
}
