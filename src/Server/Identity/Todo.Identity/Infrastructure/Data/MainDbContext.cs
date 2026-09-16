using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Infrastructure.Data
{
    public class MainDbContext : IdentityDbContext<User, Role, Guid>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
                    : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
        }
    }
}
