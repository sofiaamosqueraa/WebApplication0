using Microsoft.AspNetCore.Mvc;
using WebApplication0.Data;
using WebApplication0.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Register()
    {
        ViewBag.Companies = _context.Companies.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        ViewBag.Companies = _context.Companies.ToList();
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

            var userEmail = ViewBag.UserEmail as string;

            var user = _context.Users
                .Include(u => u.UserCompanies)
                .ThenInclude(uc => uc.Company)
                .FirstOrDefault(u => u.Email == userEmail);

            ViewBag.Companies = user?.UserCompanies.Select(uc => uc.Company).ToList();
            return View();
        }

        return RedirectToAction("Login");
    }


    public IActionResult InitializeCompanies()
    {
        var companies = new List<Company>
    {
        new Company { Name = "Lusocargo", Address = "Endereço 1" },
        new Company { Name = "Infraspeak", Address = "Endereço 2" },
        new Company { Name = "Corkart", Address = "Endereço 3" },
        new Company { Name = "BSK Medical", Address = "Endereço 4" },
        new Company { Name = "Nucase grupo", Address = "Endereço 5" }
    };

        _context.Companies.AddRange(companies);
        _context.SaveChanges();

        return Ok("Empresas inicializadas com sucesso.");
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

        // Carregar os usuários com suas empresas associadas
        var users = _context.Users
            .Include(u => u.UserCompanies)
            .ThenInclude(uc => uc.Company)
            .OrderBy(u => u.Id)
            .ToList();

        ViewBag.TotalUsers = users.Count;
        ViewBag.AdminUsers = users.Count(u => u.IsAdmin);
        ViewBag.Companies = _context.Companies.ToList();

        return View(users);
    }


    [HttpPost]
    public IActionResult AssignCompanies(int userId, List<int> companyIds)
    {
        var user = _context.Users.Include(u => u.UserCompanies).FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        user.UserCompanies.Clear();

        foreach (var companyId in companyIds)
        {
            var company = _context.Companies.Find(companyId);
            if (company != null)
            {
                user.UserCompanies.Add(new UserCompany { UserId = userId, User = user, CompanyId = companyId, Company = company });
            }
        }

        _context.SaveChanges();
        TempData["SuccessMessage"] = "Empresas atribuídas com sucesso.";
        return RedirectToAction("ListUsers");
    }

    public IActionResult CompanyDashboard(int companyId)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (userEmail == null)
        {
            return RedirectToAction("Login");
        }

        var user = _context.Users
            .Include(u => u.UserCompanies)
            .ThenInclude(uc => uc.Company)
            .FirstOrDefault(u => u.Email == userEmail);

        var company = user?.UserCompanies
            .FirstOrDefault(uc => uc.CompanyId == companyId)?.Company;

        if (company == null)
        {
            return NotFound();
        }

        ViewBag.Company = company;

        // Redirecionar para o dashboard específico da empresa
        return View($"CompanyDashboards/{company.Name.Replace(" ", "")}Dashboard");

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
