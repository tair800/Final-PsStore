using Final.Application.Profiles;
using Final.Application.Services.Implementations;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Core.Repositories;
using Final.Data.Data;
using Final.Data.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddSignalR();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // This registers IHttpClientFactory
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IPromoService, PromoService>();
builder.Services.AddScoped<IPromoRepository, PromoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<FinalDbContext>(opt =>
{
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Final.Data"));
});

builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfile(new MapperProfile(new HttpContextAccessor()));
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password = new()
    {
        RequiredLength = 8,
        RequireUppercase = true,
        RequireLowercase = true,
        RequireDigit = true,
        RequireNonAlphanumeric = true
    };

    options.Lockout = new()
    {
        MaxFailedAccessAttempts = 5,
        AllowedForNewUsers = true,
        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5)
    };

    options.User = new()
    {
        RequireUniqueEmail = true,
    };
    options.SignIn.RequireConfirmedEmail = true;

}).AddDefaultTokenProviders().AddEntityFrameworkStores<FinalDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication and Authorization Middleware (correct order)
app.UseAuthentication(); // Ensure this comes before UseAuthorization
app.UseAuthorization(); // This enables role-based authorization

// Map controller routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=DashBoard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR hub
app.MapHub<ChatHub>("/chathub");

app.Run();
