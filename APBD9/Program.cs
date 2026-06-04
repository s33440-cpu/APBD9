using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using APBD9.Data;
using Microsoft.AspNetCore.Identity;
using APBD9.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=userpanel.db"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = new PasswordHasher<AppUser>();

    context.Database.EnsureCreated();

    if (!context.AppUsers.Any(u => u.Email == "admin@test.com"))
    {
        var admin = new AppUser
        {
            Email = "admin@ukr.net",
            Role = "Admin"
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

        context.AppUsers.Add(admin);
        context.SaveChanges();
    }
}
app.Run();