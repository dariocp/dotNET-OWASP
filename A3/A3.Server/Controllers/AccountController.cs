using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using A3.Server.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace A3.Server.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login login, string returnUrl = "")
        {
            string sql = "select Id, Name, Password from [User] where Name=@Name and Password=@Password";
            User user = new SqliteConnection("Data Source=auth.db").QueryFirstOrDefault<User>(sql, new { Name = login.Username, Password = login.Password });

            if (user != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                else return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ModelState.Remove("Password");
            return View();
        }
        
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}