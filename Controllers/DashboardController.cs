using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using plant_care_hub.Models;
using plant_care_hub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace plant_care_hub.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly PlantCareHubContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public DashboardController(ILogger<DashboardController> logger, PlantCareHubContext context, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    private async Task PopulateDropdowns()
    {
        ViewBag.Especie = new SelectList(await _context.PlantSpecies.ToListAsync(), "Id", "Name");
    }

    private async Task<List<Plant>> GetUserPlants(int userId)
    {
        return await _context.Plants
            .Where(p => p.UserId == userId)
            .Include(p => p.PlantSpecies)
            .ToListAsync();
    }

    private async Task PlantHealthyStatus(int userId)
    {

        var userPlants = await _context.Plants.Where(n => n.UserId == userId).ToListAsync();

        var health = userPlants.Where(n => n.LastCareDate >= DateTime.Now.AddDays(-1)).ToList();
        var sick = userPlants.Where(n => n.LastCareDate >= DateTime.Now.AddDays(-5) && n.LastCareDate < DateTime.Now.AddDays(-1)).ToList();
        var needsAttention = userPlants.Where(n => n.LastCareDate >= DateTime.Now.AddDays(-10) && n.LastCareDate < DateTime.Now.AddDays(-5)).ToList();

        ViewBag.Healthy = health.Count;
        ViewBag.Sick = sick.Count;
        ViewBag.NeedsAttention = needsAttention.Count;
        ViewBag.TotalPlants = userPlants.Count;
    }

    private async Task<int[]> GetPlantsCreatedByMonth(int userId)
    {
        var year = DateTime.Now.Year;

        var plants = await _context.Plants
            .Where(p => p.UserId == userId && p.CreatedAt.Year == year)
            .ToListAsync();

        int[] plantsByMonth = new int[12];

        foreach (var plant in plants)
        {
            int monthIndex = plant.CreatedAt.Month - 1;
            plantsByMonth[monthIndex]++;
        }

        return plantsByMonth;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var loggedUser = _context.Users.First(x => x.Id == userId);

        ViewBag.UserName = loggedUser.Name;
        ViewBag.UserRole = loggedUser.Role;

        await PopulateDropdowns();


        var plants = await GetUserPlants(userId);
        var viewModel = new DashboardViewModel
        {
            Plants = plants,
            NewPlant = new Plant()
        };

        await PlantHealthyStatus(userId);

        ViewBag.MonthlyPlantsCount = await GetPlantsCreatedByMonth(userId);

        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var plantToEdit = await _context.Plants
            .Include(p => p.PlantSpecies)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (plantToEdit == null)
        {
            return NotFound();
        }

        await PopulateDropdowns();

        var plants = await GetUserPlants(userId);
        var viewModel = new DashboardViewModel
        {
            Plants = plants,
            NewPlant = plantToEdit
        };

        ViewBag.UserName = (await _context.Users.FindAsync(userId))?.Name;
        await PlantHealthyStatus(userId);

        ViewBag.MonthlyPlantsCount = await GetPlantsCreatedByMonth(userId);

        return View("Index", viewModel);
    }

    [HttpPost, ActionName("SavePlant")]
    public async Task<IActionResult> SavePlant(DashboardViewModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


        var plant = model.NewPlant;

        var plantToSave = await _context.Plants.FindAsync(plant.Id);

        bool newPlant = plantToSave == null;

        if (newPlant)
        {
            plantToSave = new Plant
            {
                Nome = plant.Nome,
                UserId = userId,
                CreatedAt = DateTime.Now,
                LastCareDate = DateTime.Now,
                Especie = plant.Especie,
            };
        }
        else
        {
            plantToSave.Nome = plant.Nome;
            plantToSave.Especie = plant.Especie;
            plantToSave.UpdatedAt = DateTime.Now;
        }

        if (plant.UploadedFoto != null && plant.UploadedFoto.Length > 0)
        {
            var fotoDir = Path.Combine("wwwroot", "img", "plants");
            Directory.CreateDirectory(fotoDir);
            var fileName = Guid.NewGuid() + Path.GetExtension(plant.UploadedFoto.FileName);
            var filePath = Path.Combine(fotoDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await plant.UploadedFoto.CopyToAsync(stream);
            }

            plantToSave.Foto = "/img/plants/" + fileName;

        }

        else if (newPlant)
        {
            ModelState.AddModelError("NewPlant.UploadedFoto", "Obrigatório enviar a foto");

            await PopulateDropdowns();

            var plants = await GetUserPlants(userId);
            var viewModel = new DashboardViewModel
            {
                Plants = plants,
                NewPlant = plant
            };

            return View("Index", viewModel);

        }

        if (newPlant)
        {
            _context.Plants.Add(plantToSave);
        }
        else
        {
            _context.Plants.Update(plantToSave);
        }
        await _context.SaveChangesAsync();

        if (newPlant) TempData["SuccessMessage"] = $"Planta {plantToSave.Nome} adicionada com sucesso!";
        else TempData["SuccessMessage"] = $"Planta {plantToSave.Nome} atualizada com sucesso!";

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Delete(int? id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var loggedUser = _context.Users.First(x => x.Id == userId);

        if (id == null)
        {
            return NotFound();
        }

        var plantToDelete = await _context.Plants.FirstOrDefaultAsync(m => m.Id == id);

        if (plantToDelete == null)
        {
            return NotFound();
        }
        await PopulateDropdowns();

        var plants = await GetUserPlants(userId);

        var viewModel = new DashboardViewModel
        {
            Plants = plants,
            NewPlant = new Plant(),
            PlantToDelete = plantToDelete,
            ShowDeleteConfirmation = true,
        };

        ViewBag.UserName = loggedUser.Name;
        await PlantHealthyStatus(userId);
        ViewBag.MonthlyPlantsCount = await GetPlantsCreatedByMonth(userId);

        return View("Index", viewModel);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {

        var plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);

        if (plant == null)
        {
            return NotFound();
        }


        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Planta {plant.Nome} excluída com sucesso!";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Feed()
    {
        return View();
    }
    public IActionResult Forum()
    {
        return View();
    }
    public IActionResult Library()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

