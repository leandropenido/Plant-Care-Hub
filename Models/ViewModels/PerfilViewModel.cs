namespace plant_care_hub.Models.ViewModels;

public class PerfilViewModel
{
    public User User { get; set; }
    public int FollowersNumber { get; set; }
    public int FollowingNumber { get; set; }
    public int PostNumber { get; set; }
    public int PlantsNeedAttention { get; set; }
    public int PlantsSick { get; set; }
    public int PlantsHealth { get; set; }
    public int TotalPlants { get; set; }
}
