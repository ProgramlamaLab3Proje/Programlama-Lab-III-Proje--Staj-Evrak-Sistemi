using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Admin
{
    [Authorize(Roles = StaticRoles.ROLE_ADMIN)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
