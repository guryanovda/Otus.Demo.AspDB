using Microsoft.EntityFrameworkCore;
using WebApplication2.Domain.Entity;

namespace WebApplication2.Domain
{
    public class DataContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}
