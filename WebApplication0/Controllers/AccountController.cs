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
    public IActionResult Login(User model)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

        if (user == null)
        {
            TempData["Message"] = "Não tens uma conta registada. Por favor, cria uma nova conta.";
            return RedirectToAction("Register");
        }

        if (user.Password == model.Password)
        {
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

            if (user.IsAdmin)
            {
                return RedirectToAction("ListUsers");
            }


            if (model.Email.EndsWith("@pontual.pt"))
            {
                return RedirectToAction("DashboardPontual");
            }
            else if (model.Email.EndsWith("@epfundao.edu.pt"))
            {
                return RedirectToAction("Dashboardescola");
            }
            else if (model.Email.EndsWith("@gmail.com"))
            {
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Dashboard");
        }

        ViewBag.Error = "Palavra-passe incorreta.";
        return View();
    }

    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
        {
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        return RedirectToAction("Login");
    }

    public IActionResult DashboardPontual()
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
        {
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        return RedirectToAction("Login");
    }

    public IActionResult Dashboardescola()
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
        {
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult ListUsers()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (userEmail == null)
        {
            return RedirectToAction("Login");
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
        if (user == null || !user.IsAdmin)
        {
            return Unauthorized();
        }

        var users = _context.Users.OrderBy(u => u.Id).ToList();
        ViewBag.TotalUsers = users.Count;
        ViewBag.AdminUsers = users.Count(u => u.IsAdmin);

        return View(users);
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        return RedirectToAction("ListUsers");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }



}
