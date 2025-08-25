using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("Librarys")]
    public class Library
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool Approved { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}
