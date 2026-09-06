using Microsoft.AspNetCore.Identity;

namespace Todo.Identity.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Role() : base()
        {
            CreatedAt = DateTime.UtcNow;
        }

        private Role(string name, string description) : base(name)
        {
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public static Role Create(string name, string description)
        {
            return new Role(name, description);
        }

        public void Update(string name, string description)
        {
            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
