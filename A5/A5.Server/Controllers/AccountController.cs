using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using A5.Server.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace A5.Server.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login login, string returnUrl = "")
        {
            string sql = "select * from [User] where Name=@Name and Password=@Password";
            User user = new SqliteConnection("Data Source=auth.db").QueryFirstOrDefault<User>(sql, new { Name = login.Username, Password = login.Password });

            if (user != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                Response.Cookies.Append("Group", user.Group);

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
            foreach (var cookie in Request.Cookies.Keys) Response.Cookies.Delete(cookie);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Administration()
        {
            if (Request.Cookies["Group"] == "Administrators") return View();
            else return Forbid();
        }

        public IActionResult AccessDenied() => View();
    }
}