using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Student
{
    [Authorize(Roles = StaticRoles.ROLE_STUDENT)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
