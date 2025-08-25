using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plant_care_hub.Models
{
    [Table("Plants")]
    public class Plant
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("PlantSpecies")]
        public int Especie { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Obrigatório informar o nome!")]
        public string Nome { get; set; }

        [Required]
        public string Foto { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime LastCareDate { get; set; }


        public PlantSpecies PlantSpecies { get; set; }
        public User User { get; set; }

        [NotMapped]
        public IFormFile UploadedFoto { get; set; }
    }
}