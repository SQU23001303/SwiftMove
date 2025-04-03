using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Swift_Move.Models;

namespace Swift_Move.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding Staff records here
            modelBuilder.Entity<Staff>().HasData(
                new Staff { Id = 1, FullName = "John Smith", Email = "john.smith@swiftmove.com" },
                new Staff { Id = 2, FullName = "Emily Johnson", Email = "emily.johnson@swiftmove.com" },
                new Staff { Id = 3, FullName = "Aliyah Khan", Email = "aliyah.khan@swiftmove.com" },
                new Staff { Id = 4, FullName = "David Chen", Email = "david.chen@swiftmove.com" },
                new Staff { Id = 5, FullName = "Sophie Brown", Email = "sophie.brown@swiftmove.com" },
                new Staff { Id = 6, FullName = "Carlos Garcia", Email = "carlos.garcia@swiftmove.com" },
                new Staff { Id = 7, FullName = "Liam O'Connor", Email = "liam.oconnor@swiftmove.com" },
                new Staff { Id = 8, FullName = "Zara Patel", Email = "zara.patel@swiftmove.com" },
                new Staff { Id = 9, FullName = "Noah Williams", Email = "noah.williams@swiftmove.com" },
                new Staff { Id = 10, FullName = "Hannah Lee", Email = "hannah.lee@swiftmove.com" }
            );
        }


        public DbSet<ServiceModel> Services { get; set; }




    }
}
