namespace APBD9.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using APBD9.Data;
using APBD9.Models;
using APBD9.VModels;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var notes = await _context.UserNotes
            .Where(n => n.AppUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(notes);
    }

    [HttpPost]
    public async Task<IActionResult> AddNote(NoteViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var note = new UserNote
        {
            AppUserId = userId,
            Title = model.Title,
            Content = model.Content
        };

        _context.UserNotes.Add(note);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}