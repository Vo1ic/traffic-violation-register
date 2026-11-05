using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafficRegister.Data;
using TrafficRegister.Models;

namespace TrafficRegister.Controllers;

public sealed class ViolationsController(TrafficDbContext db) : Controller
{
    public async Task<IActionResult> Index(int? inspectorId, int? articleId, ViolationStatus? status, string? q)
    {
        var query = db.Violations.AsNoTracking().Include(x => x.Vehicle).Include(x => x.Inspectors)
            .Include(x => x.Articles).AsQueryable();
        if (inspectorId.HasValue) query = query.Where(x => x.Inspectors.Any(i => i.Id == inspectorId));
        if (articleId.HasValue) query = query.Where(x => x.Articles.Any(a => a.Id == articleId));
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.CaseNumber.Contains(q) || x.Location.Contains(q) ||
                                     x.DriverName.Contains(q) || x.Vehicle!.PlateNumber.Contains(q));

        return View(new ViolationList
        {
            Items = await query.OrderByDescending(x => x.OccurredAt).ToListAsync(),
            Inspectors = await db.Inspectors.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(),
            Articles = await db.Articles.AsNoTracking().OrderBy(x => x.Code).ToListAsync(),
            InspectorId = inspectorId,
            ArticleId = articleId,
            Status = status,
            Query = q
        });
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var item = await FullQuery().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await FillOptions();
        return View("Form", new ViolationForm { CaseNumber = await NextCaseNumber(), OccurredAt = DateTime.Now });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ViolationForm form)
    {
        await ValidateReferences(form);
        if (!ModelState.IsValid)
        {
            await FillOptions(form);
            return View("Form", form);
        }

        var entity = new Violation
        {
            CaseNumber = form.CaseNumber.Trim(), OccurredAt = form.OccurredAt,
            Location = form.Location.Trim(), Description = form.Description.Trim(),
            DriverName = form.DriverName.Trim(), Status = form.Status, VehicleId = form.VehicleId,
            Inspectors = await db.Inspectors.Where(x => form.InspectorIds.Contains(x.Id)).ToListAsync(),
            Articles = await db.Articles.Where(x => form.ArticleIds.Contains(x.Id)).ToListAsync()
        };
        db.Violations.Add(entity);
        await db.SaveChangesAsync();
        TempData["Notice"] = $"Справу {entity.CaseNumber} додано до реєстру.";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var item = await FullQuery().FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        await FillOptions(new ViolationForm { Id = item.Id, CaseNumber = item.CaseNumber,
            OccurredAt = item.OccurredAt, Location = item.Location, Description = item.Description,
            DriverName = item.DriverName, Status = item.Status, VehicleId = item.VehicleId,
            InspectorIds = item.Inspectors.Select(x => x.Id).ToList(),
            ArticleIds = item.Articles.Select(x => x.Id).ToList() });
        return View("Form", new ViolationForm { Id = item.Id, CaseNumber = item.CaseNumber,
            OccurredAt = item.OccurredAt, Location = item.Location, Description = item.Description,
            DriverName = item.DriverName, Status = item.Status, VehicleId = item.VehicleId,
            InspectorIds = item.Inspectors.Select(x => x.Id).ToList(),
            ArticleIds = item.Articles.Select(x => x.Id).ToList() });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ViolationForm form)
    {
        if (id != form.Id) return NotFound();
        await ValidateReferences(form);
        if (!ModelState.IsValid)
        {
            await FillOptions(form);
            return View("Form", form);
        }

        var item = await db.Violations.Include(x => x.Inspectors).Include(x => x.Articles)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        item.CaseNumber = form.CaseNumber.Trim();
        item.OccurredAt = form.OccurredAt;
        item.Location = form.Location.Trim();
        item.Description = form.Description.Trim();
        item.DriverName = form.DriverName.Trim();
        item.Status = form.Status;
        item.VehicleId = form.VehicleId;
        item.Inspectors = await db.Inspectors.Where(x => form.InspectorIds.Contains(x.Id)).ToListAsync();
        item.Articles = await db.Articles.Where(x => form.ArticleIds.Contains(x.Id)).ToListAsync();
        await db.SaveChangesAsync();
        TempData["Notice"] = $"Справу {item.CaseNumber} оновлено.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var item = await FullQuery().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await db.Violations.FindAsync(id);
        if (item is null) return NotFound();
        var number = item.CaseNumber;
        db.Violations.Remove(item);
        await db.SaveChangesAsync();
        TempData["Notice"] = $"Справу {number} видалено.";
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<Violation> FullQuery() => db.Violations.AsNoTracking()
        .Include(x => x.Vehicle).Include(x => x.Inspectors).Include(x => x.Articles);

    private async Task<string> NextCaseNumber()
    {
        var count = await db.Violations.CountAsync() + 1;
        return $"ПДР-{DateTime.Now:yyyy}-{count:0000}";
    }

    private async Task FillOptions(ViolationForm? form = null)
    {
        ViewBag.Vehicles = await db.Vehicles.AsNoTracking().OrderBy(x => x.PlateNumber).ToListAsync();
        ViewBag.Inspectors = await db.Inspectors.AsNoTracking().OrderBy(x => x.FullName).ToListAsync();
        ViewBag.Articles = await db.Articles.AsNoTracking().OrderBy(x => x.Code).ToListAsync();
        ViewBag.SelectedInspectors = form?.InspectorIds ?? [];
        ViewBag.SelectedArticles = form?.ArticleIds ?? [];
    }

    private async Task ValidateReferences(ViolationForm form)
    {
        if (await db.Violations.AnyAsync(x => x.CaseNumber == form.CaseNumber && x.Id != form.Id))
            ModelState.AddModelError(nameof(form.CaseNumber), "Номер справи вже використовується.");
        if (form.VehicleId > 0 && !await db.Vehicles.AnyAsync(x => x.Id == form.VehicleId))
            ModelState.AddModelError(nameof(form.VehicleId), "Обраний транспортний засіб не знайдено.");
        var inspectors = form.InspectorIds.Distinct().ToList();
        var articles = form.ArticleIds.Distinct().ToList();
        if (inspectors.Count == 0 || await db.Inspectors.CountAsync(x => inspectors.Contains(x.Id)) != inspectors.Count)
            ModelState.AddModelError(nameof(form.InspectorIds), "Оберіть наявного інспектора.");
        if (articles.Count == 0 || await db.Articles.CountAsync(x => articles.Contains(x.Id)) != articles.Count)
            ModelState.AddModelError(nameof(form.ArticleIds), "Оберіть наявну статтю.");
    }
}
