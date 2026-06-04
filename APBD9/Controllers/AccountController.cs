namespace APBD9.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APBD9.Data;
using APBD9.Models;
using APBD9.VModels;
using APBD9.Exceptions;
public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existingUser = await _context.AppUsers
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        try
        {
            if (existingUser != null)
                throw new UserPanelException("User with this email already exists.");
        }
        catch (UserPanelException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        var user = new AppUser
        {
            Email = model.Email,
            Role = "User"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            model.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}