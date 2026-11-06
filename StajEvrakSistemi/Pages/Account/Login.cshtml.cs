using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Services;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims; // kullanıcının kimlik ve rol bilgileri
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StajEvrakSistemi.Utility;


namespace StajEvrakSistemi.Pages.Account;

public class LoginModel : PageModel
{
    private readonly PasswordService _passwordService;
    private readonly Context _context;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public LoginModel(PasswordService passwordService, Context context)
    {
        _passwordService = passwordService;
        _context = context;
    }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }
    public async Task<IActionResult> OnPost() //async asenkron çalışmasını sağlar, task ise yönlendirme yapacağını belirtir
    {
        StajEvrakSistemi.Models.Student? student = _context.Students.FirstOrDefault(s => s.Email == Email);
        StajEvrakSistemi.Models.Admin? admin = _context.Admins.FirstOrDefault(s => s.Email == Email);
        StajEvrakSistemi.Models.User? varsayilan = null;
        if (student != null)
        {
            varsayilan = student;
        }
        else if(admin !=null)
        {
            varsayilan = admin;
        }
        else
        {
            ErrorMessage = "Yanlış e-posta veya şifre. Lütfen tekrar deneyiniz.";
            return Page();
        }
        if (varsayilan.IsLocked)
        {
            ErrorMessage = "Bu hesap kilitlenmiştir. Kilidin kaldırılması için yöneticilerle iletişime geçiniz.";
            return Page();
        }
        else if(!_passwordService.VerifyPassword(Password, varsayilan.HashedPassword))
        {
            varsayilan.FailedLogins++;
            if (varsayilan.FailedLogins >= 5)
            {
                ErrorMessage = "Çok fazla başarısız denemede bulunduğunuz için hesabınız kilitlenmiştir. Kilidin kaldırılması için yöneticilerle iletişime geçiniz.";
                varsayilan.IsLocked = true;
            }
            else
            {
                ErrorMessage = "Yanlış e-posta veya şifre. Lütfen tekrar deneyiniz.";
            }
            _context.SaveChanges();
            return Page();
        }
        else
        {
            varsayilan.FailedLogins = 0;
            _context.SaveChanges();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , varsayilan.Id.ToString()),
                new Claim(ClaimTypes.Name , varsayilan.Email),
                new Claim(ClaimTypes.Role , varsayilan.Role)
            };

            var identity = new ClaimsIdentity(claims , CookieAuthenticationDefaults.AuthenticationScheme);
            //kullanıcı bilgilerini claims ile alıp bir identity kimlik oluşturur

            var principal = new ClaimsPrincipal(identity);
            //asp.net coreda bu kimliği temsilleyen nesnedir

            await HttpContext.SignInAsync (CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = false //cookienin kalıcı olmamasını sağlar
                }
            );
            //kimliği cookie ile tarayıcıya kaydeder. (await işlem bitene kadar durmayı, signinasync senkron girişi,
            //parantez içindeki ilk ifade cookie metodudnun seçilmesi, 2. ifade ise kimlik nesnesini belirtir.)

            return RedirectToPage("/Index");
        }
        
    }

}