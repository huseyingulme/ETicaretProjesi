using Microsoft.EntityFrameworkCore;
using ETicaret.Utils;
using ETicaret.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using ETicaret.Core.Services;
using ETicaret.Core.Interfaces;
using ETicaret.Data.Repositories;
using ETicaret.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "ETicaretSession";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.IOTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<FileHelper>();
builder.Services.AddScoped<ETicaret.Services.ICartService, ETicaret.Services.CartService>();
builder.Services.AddScoped<ETicaret.Services.IAddressService, ETicaret.Services.AddressService>();
builder.Services.AddScoped<ETicaret.Services.IOrderService, ETicaret.Services.OrderService>();

// Repository Pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Business Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "ETicaretCookie";
        options.Cookie.IsEssential = true;
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = false,
    DefaultContentType = "application/octet-stream"
});
using (var scope = app.Services.CreateScope())
{
    var fileHelper = scope.ServiceProvider.GetRequiredService<FileHelper>();
    fileHelper.EnsureDirectoriesExist();
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.EnsureCreated();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
 );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
