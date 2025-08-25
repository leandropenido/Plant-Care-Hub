using plant_care_hub.Models;

namespace plant_care_hub.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Plant> Plants { get; set; }
        public Plant NewPlant { get; set; } = new Plant();
        public Plant PlantToDelete { get; set; }
        public bool ShowDeleteConfirmation { get; set; }
    }
}