using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Services;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using System.ComponentModel.DataAnnotations;

namespace StajEvrakSistemi.Pages.Account
{
    public class RegisterModel : PageModel
    {

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Surname { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword {  get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public RegisterModel(PasswordService sifre, Context context)
        {
            _passwordService = sifre;
            _context = context;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ConfirmPassword != Password)
            {
                ErrorMessage = "Girdiğiniz şifreler eşleşmiyor. Lütfen 'Şifre' ve 'Şifre (Tekrar)' alanlarına aynı şifreyi girdiğinizden emin olunuz.";
                return Page();
            }
            else
            {
                if (Password.Length < 8)
                {
                    ErrorMessage = "Şifreniz en az 8 haneden oluşmalıdır.";
                    return Page();
                }
                else
                {
                    bool hasuppercase = false;
                    bool haslowercase = false;
                    bool hasdigit = false;
                    bool hassymbol = false;
                    for (int i = 0; i < Password.Length; i++)
                    {
                        if (char.IsUpper(Password[i]))
                        {
                            hasuppercase = true;
                        }
                        if (char.IsLower(Password[i]))
                        {
                            haslowercase = true;
                        }
                        if (char.IsDigit(Password[i]))
                        {
                            hasdigit = true;
                        }
                        if (char.IsSymbol(Password[i]) || char.IsPunctuation(Password[i]))
                        {
                            hassymbol = true;
                        }
                    }
                    if (!hasuppercase || !hassymbol || !haslowercase || !hasdigit)
                    {
                        ErrorMessage = "Şifrenizin büyük - küçük harf, sembol ve rakam içerdiğinden emin olunuz.";
                        return Page();
                    }
                }
            }
            string passwordHash = _passwordService.HashPassword(Password);

            StajEvrakSistemi.Models.Student newStudent = new StajEvrakSistemi.Models.Student
            {
                Name = Name,
                Surname = Surname,
                Email = Email,
                HashedPassword = passwordHash,
                Role = "ROLE_STUDENT"
            };
            _context.Students.Add(newStudent); // (git add gibi)
            _context.SaveChanges(); // değişiklikleri veritabanına kalıcı olarak kaydeder. (git push gibi)
            return RedirectToPage("/Account/Login");
        }

        private readonly PasswordService _passwordService;
        private readonly Context _context;
    }
}
