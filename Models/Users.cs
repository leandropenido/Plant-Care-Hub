using plant_care_hub.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static plant_care_hub.Constants.UserEnums;

namespace plant_care_hub.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage ="Obrigatório Informar o nome!")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Obrigatório Informar o email!")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Obrigatório Informar a senha!")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Obrigatório Informar o telefone!")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Obrigatório Informar o tipo de usuário!")]
    public Role Role { get; set; }

    public string? Photo { get; set; }
    public Status Status { get; set; }
    public string? Link { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    public IFormFile? UploadedPhoto { get; set; }


}

