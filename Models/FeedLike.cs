using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("FeedLikes")]
    public class FeedLike
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        [ForeignKey("Feed")]
        public int IdFeed { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Feed Feed { get; set; }
    }
}
