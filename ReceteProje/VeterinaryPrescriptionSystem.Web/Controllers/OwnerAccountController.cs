using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    public class OwnerAccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RandevuAl()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string name, string email, string phone, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Tüm zorunlu alanları doldurun.";
                return View();
            }

            using (var db = new Data.ApplicationDbContext(HttpContext.RequestServices.GetService<Microsoft.EntityFrameworkCore.DbContextOptions<Data.ApplicationDbContext>>()))
            {
                var exists = db.OwnerUsers.Any(u => u.Email == email);
                if (exists)
                {
                    TempData["Error"] = "Bu e-posta ile zaten bir hesap var.";
                    return View();
                }
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
                var hash = hasher.HashPassword(null, password);
                var owner = new Models.OwnerUser
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    PasswordHash = hash,
                    Address = ""
                };
                db.OwnerUsers.Add(owner);
                await db.SaveChangesAsync();
            }
            TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            // HIZLI GİRİŞ: Doğrudan ana panele yönlendir
            return RedirectToAction("Index", "OwnerDashboard");
            
            /*
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "E-posta ve şifre zorunludur.";
                return View();
            }
            using (var db = new Data.ApplicationDbContext(HttpContext.RequestServices.GetService<Microsoft.EntityFrameworkCore.DbContextOptions<Data.ApplicationDbContext>>()))
            {
                var user = db.OwnerUsers.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    TempData["Error"] = "Kullanıcı bulunamadı.";
                    return View();
                }
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
                var result = hasher.VerifyHashedPassword(null, user.PasswordHash, password);
                if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                {
                    TempData["Error"] = "Şifre yanlış.";
                    return View();
                }
                // Oturum aç (cookie)
                HttpContext.Session.SetInt32("OwnerUserId", user.Id);
                HttpContext.Session.SetString("OwnerUserName", user.Name);
                return RedirectToAction("Index", "OwnerDashboard");
            }
            */
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
} 