using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace BalzorAuth0.Pages
{
    public class LogOutModel : PageModel
    {
        public async Task OnGet()
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
