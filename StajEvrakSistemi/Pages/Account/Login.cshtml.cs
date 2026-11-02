using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Services;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using System.Diagnostics.Eventing.Reader;


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

    public void OnGet()
    {
    }

    public IActionResult OnPost()
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
        }

        return Page();
    }

}