using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавляем HttpClient в DI-контейнер
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Добавляем кэширование
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "parsing",
    pattern: "parsing",
    defaults: new { controller = "Telegram", action = "Index" });

app.MapControllerRoute(
        name: "vk",
        pattern: "vk/{action=GetFilteredNews}/{id?}",
        defaults: new { controller = "VK" });


app.Run();


