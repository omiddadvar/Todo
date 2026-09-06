using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Infrastructure
{
    public class MainDbContext : IdentityDbContext< User, Role, Guid>
    {
        public MainDbContext()
        {

        }
    }
}
