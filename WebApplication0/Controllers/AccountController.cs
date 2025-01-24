using Microsoft.AspNetCore.Mvc;
using WebApplication0.Data;
using WebApplication0.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

            if (user != null)
            {

                ViewBag.UserId = user.Id;
                ViewBag.Companies = user?.UserCompanies.Select(uc => uc.Company).ToList();
                return View();
            }
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


    public IActionResult UpdateUserProfile(int id, string name, string email, string newPassword)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        user.Name = name;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            user.Password = newPassword;
        }

        _context.Users.Update(user);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
        return RedirectToAction("Dashboard");
    }

    public IActionResult ConvidarUsuario()
    {
        ViewBag.Empresas = _context.Companies.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult ConvidarUsuario(string email, List<int> companyIds)
    {
        var usuarioExiste = _context.Users.Any(u => u.Email == email);
        if (usuarioExiste)
        {
            TempData["Mensagem"] = "Este e-mail já está registado.";
            return RedirectToAction("ConvidarUsuario");
        }

        var token = Guid.NewGuid().ToString();
        var convite = new Invitation
        {
            Email = email,
            Token = token,
            EmpresasSelecionadas = string.Join(",", companyIds), 
            DataCriacao = DateTime.Now 
        };

        _context.Invitations.Add(convite);
        _context.SaveChanges();

        foreach (var empresaId in companyIds)
        {
            var empresa = _context.Companies.Find(empresaId);
            if (empresa != null)
            {
                _context.UserCompanies.Add(new UserCompany { CompanyId = empresaId });
            }
        }

        _context.SaveChanges();

        var resetUrl = $"https://localhost:7130/Account/AceitarConvite/{token}";
        var emailService = new EmailService(_configuration);
              
        emailService.EnviarConvitePorEmail(email, token);

        TempData["Mensagem"] = "Convite enviado com sucesso!";
        TempData["ShowModal"] = "true";
        TempData["Email"] = email;
        TempData["Token"] = token;
        return RedirectToAction("ConvidarUsuario");
    }

    [HttpPost]
    public IActionResult AssociarEmpresas(string token, List<int> companyIds)
    {
        var convite = _context.Invitations.FirstOrDefault(i => i.Token == token);
        if (convite != null)
        {
            convite.EmpresasSelecionadas = string.Join(",", companyIds);
            _context.SaveChanges();
            TempData["Mensagem"] = "Empresas associadas com sucesso!";
        }
        else
        {
            TempData["Mensagem"] = "Convite não encontrado.";
        }
        return RedirectToAction("ConvidarUsuario");
    }


    [HttpGet]
    public IActionResult AceitarConvite(string token)
    {
        var convite = _context.Invitations.FirstOrDefault(i => i.Token == token);
        if (convite == null || (DateTime.Now - convite.DataCriacao).TotalMinutes > 5)
        {
            TempData["MensagemErro"] = "Este convite é inválido ou expirou.";
            return RedirectToAction("Erro", "Home");
        }

        ViewBag.Token = token;
        return View();
    }


    [HttpPost]
    public IActionResult AceitarConvite(string token, string name, string password)
    {
        var convite = _context.Invitations.FirstOrDefault(i => i.Token == token);
        if (convite == null)
        {
            TempData["MensagemErro"] = "Este convite é inválido.";
            return RedirectToAction("Erro", "Home");
        }

        var usuario = new User
        {
            Email = convite.Email,
            Password = password,
            Name = name
        };

        _context.Users.Add(usuario);
        _context.SaveChanges();

        var empresasIds = convite.EmpresasSelecionadas.Split(',').Select(int.Parse).ToList();
        foreach (var empresaId in empresasIds)
        {
            var empresa = _context.Companies.Find(empresaId);
            if (empresa != null)
            {
                usuario.UserCompanies.Add(new UserCompany { UserId = usuario.Id, CompanyId = empresaId });
            }
        }

        _context.SaveChanges();
        _context.Invitations.Remove(convite);
        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Conta criada com sucesso! Agora pode iniciar sessão.";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

}