using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Student;

// Bu sayfaya yalnızca öğrenci rolüne sahip kullanıcılar erişebilir.
[Authorize(Roles = StaticRoles.ROLE_STUDENT)]
public class ApplicationsModel : PageModel
{
    // Veritabanına erişebilmek için Context nesnesini tutar.
    private readonly Context _context;

    // Sayfaya ait başvuru kayıtlarını veritabanından alır.
    public ApplicationsModel(Context context)
    {
        _context = context;
    }

    // Giriş yapan öğrencinin başvurularını ekranda göstermek için tutar.
    public List<StudentRecord> Applications { get; set; } = new();

    // Öğrenci başvuru sayfasını açtığında çalışır.
    public async Task OnGetAsync()
    {
        // Giriş yapan kullanıcının kimlik numarasını cookie içindeki claim bilgisinden alır.
        var studentIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Kimlik numarası geçerli bir tam sayıya dönüştürülemiyorsa işlemi durdurur.
        if (!int.TryParse(studentIdValue, out int studentId))
        {
            return;
        }

        // Veritabanından yalnızca giriş yapan öğrenciye ait başvuruları getirir.
        // Başvuruları oluşturulma tarihine göre yeniden eskiye sıralar.
        Applications = await _context.StudentRecords
            .Where(record => record.StudentId == studentId)
            .OrderByDescending(record => record.CreatedAt)
            .ToListAsync();
    }
}
