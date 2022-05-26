namespace Social_Media_v3.Data
{
    using Microsoft.EntityFrameworkCore;
    using Social_Media_v3.Models;
    public class SocialMediaDbContext : DbContext
    {
        public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options) : base(options)
        {

        }

        public DbSet<User_v3> Users { get; set; }

    }
}