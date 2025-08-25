using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("Feed")]
    public class Feed
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public string VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}
