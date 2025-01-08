using Microsoft.AspNetCore.Mvc;
using WebApplication0.Data;
using WebApplication0.Models;
using System.Linq;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        return View(user);
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        // Verificar si el usuario existe
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            // Si no existe, redirigir al registro
            TempData["Message"] = "No tienes una cuenta registrada. Por favor, crea una nueva cuenta.";
            return RedirectToAction("Register");
        }

        // Verificar contraseña
        if (user.Password == password)
        {
            TempData["UserEmail"] = user.Email;
            TempData["UserName"] = user.Name;

            return RedirectToAction("Dashboard");
        }

        ViewBag.Error = "Contraseña incorrecta.";
        return View();
    }

    public IActionResult Dashboard()
    {
        if (TempData["UserEmail"] != null)
        {
            ViewBag.UserEmail = TempData["UserEmail"];
            ViewBag.UserName = TempData["UserName"];
            return View();
        }

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ListUsers()
    {
        var users = _context.Users.ToList();
        return View(users);
    }
}
