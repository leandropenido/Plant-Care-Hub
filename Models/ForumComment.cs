using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("ForumComments")]
    public class ForumComment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        [ForeignKey("Forum")]
        public int IdForum { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Forum Forum { get; set; }
    }
}
