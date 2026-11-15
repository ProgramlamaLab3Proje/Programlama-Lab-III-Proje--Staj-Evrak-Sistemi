using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Utility;
using Microsoft.AspNetCore.Mvc;

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


    // Öğrencinin seçtiği belgeyi görüntülemek için çalışır.
    public async Task<IActionResult> OnGetDocumentAsync(int id)
    {
        // Giriş yapan öğrencinin kimlik numarasını alır.
        var studentIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Kimlik numarası geçerli değilse erişimi engeller.
        if (!int.TryParse(studentIdValue, out int studentId))
        {
            return Forbid();
        }

        // Yalnızca giriş yapan öğrenciye ait başvuruyu bulur.
        var application = await _context.StudentRecords
            .FirstOrDefaultAsync(record =>
                record.Id == id && record.StudentId == studentId);

        // Başvuru bulunamazsa dosyaya erişim verilmez.
        if (application == null || string.IsNullOrEmpty(application.DocumentPath))
        {
            return NotFound();
        }

        // Dosya adından klasör bilgilerini temizleyerek güvenli bir yol oluşturur.
        string fileName = Path.GetFileName(application.DocumentPath);

        string filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "UploadedDocuments",
            fileName
        );

        // Dosya fiziksel olarak bulunmuyorsa hata döndürür.
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        // Dosyanın uzantısına göre tarayıcıya uygun içerik türünü belirler.
        string contentType = Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        // Dosyayı tarayıcıya gönderir.
        return new PhysicalFileResult(filePath, contentType);
    }

}
