using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages;

[Authorize(Roles = StaticRoles.ROLE_STUDENT)]
public class StudentRecordModel : PageModel
{
    private readonly InternshipCalendarService _calendarService = new();
    private readonly Context _context;
    private string? _savedFileName;

    public StudentRecordModel(Context context)
    {
        _context = context;
    }

    [BindProperty]
    public DateTime? StartDate { get; set; }

    [BindProperty]
    public DateTime? EndDate { get; set; }

    [BindProperty]
    public DateTime? SubmissionDate { get; set; }

    [BindProperty]
    public StudentRecord StudentRecord { get; set; } = new();

    [BindProperty]
    public List<string> SelectedDays { get; set; } = new();

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    public string UploadMessage { get; set; } = "";
    public bool IsUploadSuccess { get; set; }

    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB
    private readonly string[] allowedExtensions =
    {
        ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png"
    };

    // Hesaplama sonucunu arayüze taşımak için
    public CalendarCalculationResult? CalculationResult { get; set; }
    public List<DocumentFieldComparison> ComparisonResults { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // temel tarihlerin girilip girilmediğini kontrol etmek için
        if (StartDate.HasValue && EndDate.HasValue && SubmissionDate.HasValue)
        {
            // seçilen günleri DayOfWeek türüne çevir
            var selectedDaysEnum = SelectedDays
                .Select(d => Enum.Parse<DayOfWeek>(d))
                .ToList();

            // takvim hesap motorunu çalıştır
            CalculationResult = _calendarService.Calculate(
                StartDate.Value,
                EndDate.Value,
                selectedDaysEnum,
                SubmissionDate.Value
            );
        }

        return Page();
    }

    // mevcut dosya doğrulama ve kaydetme metodunu çağırıyor
    public async Task<IActionResult> OnPostSaveAsync()
    {
        // formdaki bilgiler eksik veya hatalıysa kaydetmeden geri döner
        if (!ModelState.IsValid)
        {
            UploadMessage = "Lütfen öğrenci bilgilerini eksiksiz ve doğru formatta doldurduğunuzdan emin olunuz.";
            IsUploadSuccess = false;
            return Page();
        }

        await ValidateAndSaveEk16Async();
        if (!IsUploadSuccess)
            return Page();

        // sunucuya kaydetme işlemleri
        StudentRecord.Id = 0;
        StudentRecord.StudentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        StudentRecord.DocumentPath = _savedFileName;
        StudentRecord.Status = BelgeDurumu.Yuklendi;
        StudentRecord.CreatedAt = DateTime.Now;

        _context.StudentRecords.Add(StudentRecord);
        await _context.SaveChangesAsync();

        UploadMessage += " Başvurunuz kaydedildi.";
        return Page();
    }
    private async Task ValidateAndSaveEk16Async()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            UploadMessage = "Lütfen bir dosya seçiniz.";
            IsUploadSuccess = false;
            return;
        }

        if (UploadedFile.Length > MaxFileSize)
        {
            UploadMessage = "Dosya boyutu 20 MB sınırını aşamaz.";
            IsUploadSuccess = false;
            return;
        }

        string extension = Path
            .GetExtension(UploadedFile.FileName)
            .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            UploadMessage = "Geçersiz dosya formatı. Lütfen DOC, DOCX, PDF, JPG, JPEG veya PNG yükleyiniz.";
            IsUploadSuccess = false;
            return;
        }

        if (!DocumentFormatValidator.IsValid(UploadedFile))
        {
            UploadMessage = "Dosyanın gerçek formatı, dosya uzantısıyla eşleşmiyor veya dosya geçersiz.";
            IsUploadSuccess = false;
            return;
        }

        string uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "UploadedDocuments"
        );

        Directory.CreateDirectory(uploadFolder);

        string safeFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(uploadFolder, safeFileName);

        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                await UploadedFile.CopyToAsync(fileStream);
            }

            _savedFileName = safeFileName;

            // PDF veya DOCX dosyasındaki metni okur ve öğrenci alanlarını çıkarır.
            if (extension == ".pdf" || extension == ".docx")
            {
                string extractedText = DocumentTextExtractor.ExtractText(filePath);

                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    UploadMessage = "Dosya yüklendi ancak metin okunamadı.";
                }
                else
                {
                    var extractedFields =
                        DocumentFieldExtractor.ExtractStudentFields(extractedText);

                    ComparisonResults = DocumentComparisonService.Compare(
                        extractedFields,
                        StudentRecord
                    );

                    UploadMessage = "Dosya yüklendi. Metin okundu ve öğrenci alanları çıkarma işlemi tamamlandı.";
                }
            }
            else
            {
                UploadMessage = "Dosya yüklendi. Bu format için metin okuma henüz yapılmıyor.";
            }

            IsUploadSuccess = true;
        }
        catch (IOException)
        {
            UploadMessage = "Dosya kaydedilirken bir hata oluştu.";
            IsUploadSuccess = false;
        }
        catch (UnauthorizedAccessException)
        {
            UploadMessage = "Dosya kaydedilemedi. Klasöre erişim izni bulunmuyor.";
            IsUploadSuccess = false;
        }
    }
}