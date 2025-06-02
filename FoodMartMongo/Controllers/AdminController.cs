using FoodMartMongo.Entities;
using FoodMartMongo.Services.AdminServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodMartMongo.Controllers
{
    [AllowAnonymous] // Bu controller'daki tüm aksiyonlar anonim erişime açık (giriş yapmamış kullanıcılar da erişebilir)
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre gerekli.");
                return View(user);
            }

            await _adminService.RegisterUserAsync(user);
            TempData["SuccessMessage"] = "Kayıt başarılı, giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _adminService.GetUserByUsernameAsync(username);
            if (user == null || !await _adminService.CheckPasswordAsync(user, password))
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
                return View();
            }

            // Kullanıcı için Claims oluşturuyoruz
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // İstersen burada oturumun süresi gibi ayarlar yapabilirsin
                IsPersistent = true
            };

            // Kullanıcıyı sisteme giriş yapmış olarak işaretle
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("CategoryList", "Category");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Admin");
        }
    }
}
