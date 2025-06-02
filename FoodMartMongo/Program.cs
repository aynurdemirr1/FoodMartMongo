using FoodMartMongo.Services.AdminServices;
using FoodMartMongo.Services.CategoryServices;
using FoodMartMongo.Services.CustomerServices;
using FoodMartMongo.Services.DiscountServices;
using FoodMartMongo.Services.FeatureServices;
using FoodMartMongo.Services.PeopleViewingServices;
using FoodMartMongo.Services.ProductServices;
using FoodMartMongo.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Servis bağımlılıkları
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IPeopleViewingService, PeopleViewingService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Veritabanı ayarları
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettingsKey"));
builder.Services.AddScoped<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

// Authentication (Cookie tabanlı kimlik doğrulama)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login"; // Giriş sayfası
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum süresi
        options.SlidingExpiration = true;
    });

// Session ve MVC
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ortam bazlı hata yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 Authentication & Authorization sırası önemli
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
