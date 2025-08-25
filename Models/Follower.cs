using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("Followers")]
    public class Follower
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(FollowerUser))]
        public int IdFollower { get; set; }

        [ForeignKey(nameof(FollowingUser))]
        public int IdFollowing { get; set; }

        public DateTime CreatedAt { get; set; }

        public User FollowerUser { get; set; }
        public User FollowingUser { get; set; }
    }
}
