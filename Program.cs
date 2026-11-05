using Microsoft.EntityFrameworkCore;
using TrafficRegister.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TrafficDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TrafficDb")));

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrafficDbContext>();
    await db.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(db);
}

await app.RunAsync();
