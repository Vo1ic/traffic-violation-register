using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafficRegister.Data;
using TrafficRegister.Models;

namespace TrafficRegister.Controllers;

public sealed class CatalogController(TrafficDbContext db) : Controller
{
    private static readonly Dictionary<string, string> Titles = new()
    {
        ["inspectors"] = "Інспектори",
        ["vehicles"] = "Транспорт",
        ["articles"] = "Статті порушень"
    };

    public async Task<IActionResult> Index(string kind = "inspectors")
    {
        if (!Titles.ContainsKey(kind)) return NotFound();
        ViewBag.Kind = kind;
        ViewBag.Title = Titles[kind];
        var rows = kind switch
        {
            "inspectors" => await db.Inspectors.AsNoTracking().OrderBy(x => x.FullName)
                .Select(x => new CatalogRow(x.Id, x.FullName, x.BadgeNumber, $"{x.Rank} · {x.Unit}", null, null)).ToListAsync(),
            "vehicles" => await db.Vehicles.AsNoTracking().OrderBy(x => x.PlateNumber)
                .Select(x => new CatalogRow(x.Id, x.PlateNumber, x.MakeModel, x.OwnerName, null, x.Year)).ToListAsync(),
            _ => await db.Articles.AsNoTracking().OrderBy(x => x.Code)
                .Select(x => new CatalogRow(x.Id, x.Code, x.Title, x.Category, x.FineAmount, null)).ToListAsync()
        };
        return View(rows);
    }

    public IActionResult Create(string kind)
    {
        if (!Titles.ContainsKey(kind)) return NotFound();
        ViewBag.Title = $"Додати: {Titles[kind]}";
        return View("Form", new CatalogForm { Kind = kind });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CatalogForm form)
    {
        if (!Titles.ContainsKey(form.Kind)) return NotFound();
        if (form.Kind == "articles" && form.Amount is null)
            ModelState.AddModelError(nameof(form.Amount), "Вкажіть суму штрафу.");
        if (!ModelState.IsValid)
        {
            ViewBag.Title = $"Додати: {Titles[form.Kind]}";
            return View("Form", form);
        }
        AddEntity(form);
        await db.SaveChangesAsync();
        TempData["Notice"] = "Запис додано до довідника.";
        return RedirectToAction(nameof(Index), new { kind = form.Kind });
    }

    public async Task<IActionResult> Edit(string kind, int? id)
    {
        if (!Titles.ContainsKey(kind) || id is null) return NotFound();
        var form = await ToForm(kind, id.Value);
        if (form is null) return NotFound();
        ViewBag.Title = $"Редагувати: {Titles[kind]}";
        return View("Form", form);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CatalogForm form)
    {
        if (!Titles.ContainsKey(form.Kind) || id != form.Id) return NotFound();
        if (form.Kind == "articles" && form.Amount is null)
            ModelState.AddModelError(nameof(form.Amount), "Вкажіть суму штрафу.");
        if (!ModelState.IsValid)
        {
            ViewBag.Title = $"Редагувати: {Titles[form.Kind]}";
            return View("Form", form);
        }
        switch (form.Kind)
        {
            case "inspectors":
                var officer = await db.Inspectors.FindAsync(id);
                if (officer is null) return NotFound();
                officer.FullName = form.Name.Trim(); officer.BadgeNumber = form.Sub.Trim();
                officer.Rank = form.Extra?.Trim() ?? "Інспектор"; officer.Unit = form.Fourth?.Trim() ?? "";
                break;
            case "vehicles":
                var vehicle = await db.Vehicles.FindAsync(id);
                if (vehicle is null) return NotFound();
                vehicle.PlateNumber = form.Name.Trim(); vehicle.MakeModel = form.Sub.Trim();
                vehicle.OwnerName = form.Extra?.Trim() ?? ""; vehicle.Year = form.Year;
                break;
            default:
                var article = await db.Articles.FindAsync(id);
                if (article is null) return NotFound();
                article.Code = form.Name.Trim(); article.Title = form.Sub.Trim();
                article.Category = form.Extra?.Trim() ?? "Загальна"; article.FineAmount = form.Amount ?? 0;
                break;
        }
        await db.SaveChangesAsync();
        TempData["Notice"] = "Запис довідника оновлено.";
        return RedirectToAction(nameof(Index), new { kind = form.Kind });
    }

    public async Task<IActionResult> Delete(string kind, int? id)
    {
        if (!Titles.ContainsKey(kind) || id is null) return NotFound();
        var form = await ToForm(kind, id.Value);
        if (form is null) return NotFound();
        ViewBag.Title = $"Видалити: {Titles[kind]}";
        return View(form);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string kind, int id)
    {
        if (!Titles.ContainsKey(kind)) return NotFound();
        var linked = kind switch
        {
            "inspectors" => await db.Inspectors.AnyAsync(x => x.Id == id && x.Violations.Any()),
            "vehicles" => await db.Vehicles.AnyAsync(x => x.Id == id && x.Violations.Any()),
            _ => await db.Articles.AnyAsync(x => x.Id == id && x.Violations.Any())
        };
        if (linked)
        {
            TempData["Notice"] = "Запис пов'язаний зі справою та не видалений. Спочатку змініть або видаліть справу.";
            return RedirectToAction(nameof(Index), new { kind });
        }
        switch (kind)
        {
            case "inspectors":
                var officer = await db.Inspectors.FindAsync(id);
                if (officer is not null) db.Inspectors.Remove(officer);
                break;
            case "vehicles":
                var vehicle = await db.Vehicles.FindAsync(id);
                if (vehicle is not null) db.Vehicles.Remove(vehicle);
                break;
            default:
                var article = await db.Articles.FindAsync(id);
                if (article is not null) db.Articles.Remove(article);
                break;
        }
        await db.SaveChangesAsync();
        TempData["Notice"] = "Запис довідника видалено.";
        return RedirectToAction(nameof(Index), new { kind });
    }

    private void AddEntity(CatalogForm f)
    {
        switch (f.Kind)
        {
            case "inspectors": db.Inspectors.Add(new Inspector { FullName = f.Name.Trim(), BadgeNumber = f.Sub.Trim(), Rank = f.Extra?.Trim() ?? "Інспектор", Unit = f.Fourth?.Trim() ?? "Підрозділ не вказано" }); break;
            case "vehicles": db.Vehicles.Add(new Vehicle { PlateNumber = f.Name.Trim(), MakeModel = f.Sub.Trim(), OwnerName = f.Extra?.Trim() ?? "", Year = f.Year }); break;
            case "articles": db.Articles.Add(new OffenseArticle { Code = f.Name.Trim(), Title = f.Sub.Trim(), Category = f.Extra?.Trim() ?? "Загальна", FineAmount = f.Amount ?? 0 }); break;
        }
    }

    private async Task<CatalogForm?> ToForm(string kind, int id) => kind switch
    {
        "inspectors" => await db.Inspectors.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new CatalogForm { Kind = kind, Id = x.Id, Name = x.FullName, Sub = x.BadgeNumber, Extra = x.Rank, Fourth = x.Unit }).FirstOrDefaultAsync(),
        "vehicles" => await db.Vehicles.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new CatalogForm { Kind = kind, Id = x.Id, Name = x.PlateNumber, Sub = x.MakeModel, Extra = x.OwnerName, Year = x.Year }).FirstOrDefaultAsync(),
        "articles" => await db.Articles.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new CatalogForm { Kind = kind, Id = x.Id, Name = x.Code, Sub = x.Title, Extra = x.Category, Amount = x.FineAmount }).FirstOrDefaultAsync(),
        _ => null
    };
}
