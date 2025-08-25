using Microsoft.EntityFrameworkCore;

namespace plant_care_hub.Models
{
    public class PlantCareHubContext : DbContext
    {
        public PlantCareHubContext(DbContextOptions<PlantCareHubContext> options): base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<PlantSpecies> PlantSpecies { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Library> Library { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }
        public DbSet<ForumCategory> ForumCategories { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedLike> FeedLikes { get; set; }
        public DbSet<FeedComment> FeedComments { get; set; }
        public DbSet<Forum> Forums { get; set; }
    }
}
