using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("Forum")]
    public class Forum
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        [ForeignKey("ForumCategory")]
        public int Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public ForumCategory ForumCategory { get; set; }
    }
}
