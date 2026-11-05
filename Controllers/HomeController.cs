using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafficRegister.Data;

namespace TrafficRegister.Controllers;

public sealed class HomeController(TrafficDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Total = await db.Violations.CountAsync();
        ViewBag.Active = await db.Violations.CountAsync(x => x.Status != Models.ViolationStatus.Closed);
        ViewBag.Inspectors = await db.Inspectors.CountAsync();
        var all = await db.Violations.Include(x => x.Vehicle).Include(x => x.Inspectors)
            .Include(x => x.Articles).OrderByDescending(x => x.OccurredAt).ToListAsync();
        ViewBag.Fines = all.SelectMany(x => x.Articles).Sum(x => x.FineAmount);
        var recent = all.Take(5).ToList();
        return View(recent);
    }

    public IActionResult Error() => View();
}
