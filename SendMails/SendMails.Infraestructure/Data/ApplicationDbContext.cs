using Microsoft.EntityFrameworkCore;
using SendMails.Domain.Entities;

namespace SendMails.Infraestructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lista> Lista { get; set; }

    }
}
