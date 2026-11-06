using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Student;

[Authorize(Roles = StaticRoles.ROLE_STUDENT)]
public class ApplicationsModel : PageModel
{
    public void OnGet() { }
}
