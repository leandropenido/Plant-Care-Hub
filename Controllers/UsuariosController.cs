using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plant_care_hub.Models;
using System.Security.Claims;
using static plant_care_hub.Constants.UserEnums;

namespace plant_care_hub.Controllers;

public class UsuariosController : Controller
{
    private readonly PlantCareHubContext _context;

    public UsuariosController(PlantCareHubContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([Bind("Email,Password")] User usuario)
    {
        var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == usuario.Email);

        if (user == null)
        {
            ViewBag.Message = "Usuário e/ou Senha inválidos!";
            return View();
        }

        bool validatePassword = usuario.Password.Equals(user.Password);

        if (validatePassword)
        {
            var photoPath = string.IsNullOrEmpty(user.Photo) ? "/img/users/default.png" : user.Photo;

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("photo_url", photoPath)
                };

            var userIdentity = new ClaimsIdentity(claims, "login");

            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

            var props = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.ToLocalTime().AddDays(7),
                IsPersistent = true
            };
            await HttpContext.SignInAsync(principal, props);
            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Message = "Usuário e/ou Senha inválidos!";
        return Redirect("/");
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User usuario)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Este email já está em uso.");
                return RedirectToAction("Login", "Usuarios");
            }
            usuario.CreatedAt = DateTime.Now;
            usuario.Status = Status.Active;

            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Usuarios");
        }
        return RedirectToAction("Login", "Usuarios");
    }

    public async Task<IActionResult> Edit()
    {

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User usuario)
    {
        var userToUpdate = await _context.Users.FindAsync(usuario.Id);
        var nomeArquivo = Path.GetFileName(usuario.UploadedPhoto?.FileName);
        var caminho = Path.Combine("wwwroot/img/users", nomeArquivo);
        Directory.CreateDirectory(Path.GetDirectoryName(caminho));

        userToUpdate.Photo = "/img/users/" + nomeArquivo;
        userToUpdate.Name = usuario.Name;
        userToUpdate.PhoneNumber = usuario.PhoneNumber;
        userToUpdate.Bio = usuario.Bio;
        userToUpdate.Link = usuario.Link;

        using (var stream = new FileStream(caminho, FileMode.Create))
        {
            await usuario.UploadedPhoto.CopyToAsync(stream);
        }

        _context.Update(userToUpdate);
        await _context.SaveChangesAsync();

        // Atualiza a claim de photo_url se o usuário estiver autenticado
        if (User.Identity.IsAuthenticated)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userToUpdate.Name),
                new Claim(ClaimTypes.NameIdentifier, userToUpdate.Id.ToString()),
                new Claim(ClaimTypes.Role, userToUpdate.Role.ToString()),
                new Claim("photo_url", userToUpdate.Photo ?? "/img/users/default.png")
            };

            var userIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(principal);
        }

        return RedirectToAction(nameof(Edit));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Users.FindAsync(id);
        _context.Users.Remove(usuario);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login", "Usuarios");
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Users.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    public IActionResult EsqueciSenha()
    {
        return View();
    }

    public IActionResult ModificarSenha()
    {
        return View();
    }

    public IActionResult MensagemEmail()
    {
        return View();
    }
}
