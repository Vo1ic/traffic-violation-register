using Microsoft.EntityFrameworkCore;
using TrafficRegister.Models;

namespace TrafficRegister.Data;

public static class SeedData
{
    public static async Task InitializeAsync(TrafficDbContext db)
    {
        if (await db.Violations.AnyAsync()) return;

        var officers = new[]
        {
            new Inspector { FullName = "Олександр Коваль", BadgeNumber = "АІ-1042", Rank = "Старший лейтенант", Unit = "Патрульний сектор №1" },
            new Inspector { FullName = "Ірина Мельник", BadgeNumber = "АІ-1178", Rank = "Лейтенант", Unit = "Патрульний сектор №2" },
            new Inspector { FullName = "Дмитро Шевченко", BadgeNumber = "АІ-1216", Rank = "Капітан", Unit = "Відділ оформлення матеріалів" },
            new Inspector { FullName = "Марія Бондар", BadgeNumber = "АІ-1325", Rank = "Лейтенант", Unit = "Патрульний сектор №1" }
        };
        var vehicles = new[]
        {
            new Vehicle { PlateNumber = "АА1234КХ", MakeModel = "Toyota Corolla", OwnerName = "Андрій Лисенко", Year = 2018 },
            new Vehicle { PlateNumber = "КА5678ІМ", MakeModel = "Skoda Octavia", OwnerName = "Олена Ткаченко", Year = 2021 },
            new Vehicle { PlateNumber = "ВІ9012СО", MakeModel = "Volkswagen Golf", OwnerName = "Сергій Кравець", Year = 2016 },
            new Vehicle { PlateNumber = "АІ3456РТ", MakeModel = "Renault Megane", OwnerName = "Наталія Романюк", Year = 2020 },
            new Vehicle { PlateNumber = "КС7890НЕ", MakeModel = "Hyundai Tucson", OwnerName = "Максим Бойко", Year = 2022 }
        };
        var articles = new[]
        {
            new OffenseArticle { Code = "12.1", Title = "Перевищення встановленої швидкості", FineAmount = 340, Category = "Швидкість" },
            new OffenseArticle { Code = "14.2", Title = "Порушення правил проїзду перехрестя", FineAmount = 510, Category = "Маневрування" },
            new OffenseArticle { Code = "18.4", Title = "Порушення вимог дорожніх знаків", FineAmount = 340, Category = "Знаки" },
            new OffenseArticle { Code = "21.3", Title = "Користування телефоном під час руху", FineAmount = 510, Category = "Безпека" },
            new OffenseArticle { Code = "25.1", Title = "Порушення правил паркування", FineAmount = 680, Category = "Зупинка і стоянка" },
            new OffenseArticle { Code = "31.2", Title = "Непристебнутий ремінь безпеки", FineAmount = 510, Category = "Безпека" }
        };
        db.AddRange(officers);
        db.AddRange(vehicles);
        db.AddRange(articles);
        await db.SaveChangesAsync();

        var now = DateTime.Now;
        db.Violations.AddRange(
            new Violation { CaseNumber = "ПДР-2026-0104", OccurredAt = now.AddHours(-2), Location = "вул. Хрещатик, 22", DriverName = "Андрій Лисенко", Description = "Швидкість зафіксовано засобом контролю руху.", Status = ViolationStatus.Issued, Vehicle = vehicles[0], Inspectors = [officers[0], officers[1]], Articles = [articles[0]] },
            new Violation { CaseNumber = "ПДР-2026-0103", OccurredAt = now.AddDays(-1), Location = "просп. Перемоги, 48", DriverName = "Олена Ткаченко", Description = "Проїзд перехрестя на заборонний сигнал.", Status = ViolationStatus.Appealed, Vehicle = vehicles[1], Inspectors = [officers[2]], Articles = [articles[1]] },
            new Violation { CaseNumber = "ПДР-2026-0102", OccurredAt = now.AddDays(-2), Location = "вул. Січових Стрільців, 9", DriverName = "Сергій Кравець", Description = "Автомобіль залишено в зоні дії знака.", Status = ViolationStatus.Registered, Vehicle = vehicles[2], Inspectors = [officers[3]], Articles = [articles[2], articles[4]] },
            new Violation { CaseNumber = "ПДР-2026-0101", OccurredAt = now.AddDays(-3), Location = "вул. Грушевського, 16", DriverName = "Наталія Романюк", Description = "Водій користувався телефоном під час руху.", Status = ViolationStatus.Closed, Vehicle = vehicles[3], Inspectors = [officers[0]], Articles = [articles[3]] },
            new Violation { CaseNumber = "ПДР-2026-0100", OccurredAt = now.AddDays(-4), Location = "вул. Лесі Українки, 31", DriverName = "Максим Бойко", Description = "Порушення вимог щодо використання ременя безпеки.", Status = ViolationStatus.Issued, Vehicle = vehicles[4], Inspectors = [officers[1], officers[3]], Articles = [articles[5]] }
        );
        await db.SaveChangesAsync();
    }
}
