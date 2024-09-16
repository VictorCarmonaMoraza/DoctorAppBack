using Microsoft.EntityFrameworkCore;
using MODEL.Entity;

namespace DATA
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
