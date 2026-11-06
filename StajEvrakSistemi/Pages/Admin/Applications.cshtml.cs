using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Admin;

[Authorize(Roles = StaticRoles.ROLE_ADMIN)]
public class ApplicationsModel : PageModel
{
    public void OnGet() { }
}
