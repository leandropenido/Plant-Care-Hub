using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_care_hub.Models;
using plant_care_hub.Models.ViewModels;
using System.Security.Claims;

namespace plant_care_hub.Controllers;

public class PerfilController : Controller
{
    private readonly PlantCareHubContext _context;

    public PerfilController(PlantCareHubContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _context.Users.FindAsync(userId);

        var followers = await _context.Followers
            .Include(f => f.FollowerUser)
            .Where(f => f.IdFollowing == userId)
            .CountAsync();

        var following = await _context.Followers
                        .Include(f => f.FollowingUser)
                        .Where(f => f.IdFollower == userId)
                        .CountAsync();

        var postNumber  = await _context.Feeds
            .Where(p => p.IdUser == userId)
            .CountAsync();

        var userPlants = await _context.Plants.Where(n => n.UserId == user.Id).ToListAsync();

        var health = userPlants.Where(n => n.LastCareDate >= DateTime.Now.AddDays(-1)).ToList();
        var sick = userPlants.Where(n => n.LastCareDate >= DateTime.Now.AddDays(-5) && n.LastCareDate < DateTime.Now.AddDays(-1)).ToList();
        var needsAttention = userPlants.Where(n => n.LastCareDate >= DateTime.Now.AddDays(-10) && n.LastCareDate < DateTime.Now.AddDays(-5)).ToList();

        var perfilViewModel = new PerfilViewModel
        {
            User = user,
            FollowersNumber = followers,
            FollowingNumber = following,
            PostNumber = postNumber,
            PlantsHealth = health.Count(),
            PlantsSick = sick.Count(),
            PlantsNeedAttention = needsAttention.Count(),
            TotalPlants = userPlants.Count()
        };

        return View(perfilViewModel);
    }
}
