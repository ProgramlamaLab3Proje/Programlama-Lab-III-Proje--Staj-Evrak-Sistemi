using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Admin;

[Authorize(Roles = StaticRoles.ROLE_ADMIN)]
public class StudentRecordsModel : PageModel
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const int ColumnCount = 13;
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    private readonly Context _context;

    public StudentRecordsModel(Context context)
    {
        _context = context;
    }

    [BindProperty]
    public IFormFile? CsvFile { get; set; }

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // dosya yükleme kontrollerini yapar
        if (CsvFile == null || CsvFile.Length == 0)
        {
            Message = "Lütfen bir CSV dosyası seçiniz.";
            IsSuccess = false;
            return Page();
        }

        string extension = Path.GetExtension(CsvFile.FileName).ToLowerInvariant();
        if (extension != ".csv")
        {
            Message = "Yalnızca .csv uzantılı dosyalar kabul edilir.";
            IsSuccess = false;
            return Page();
        }

        if (CsvFile.Length > MaxFileSize)
        {
            Message = "CSV dosyası 5 MB sınırını aşamaz.";
            IsSuccess = false;
            return Page();
        }

        // dosyayı okur
        using var reader = new StreamReader(CsvFile.OpenReadStream());

        string? header = await reader.ReadLineAsync();
        if (header == null)
        {
            Message = "Dosyada başlık satırı bulunamadı.";
            IsSuccess = false;
            return Page();
        }

        // veritabanında kayıtlı olan öğrenci numaealarını alır
        var existingNumbers = (await _context.CSVRecords
            .Select(r => r.StudentNumber)
            .ToListAsync()).ToHashSet();

        int added = 0;
        int lineNo = 1;
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNo++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // sütunlara ayırır boşluk temizler
            string[] c = line.Split(';')
                             .Select(x => x.Trim())
                             .ToArray();

            if (c.Length != ColumnCount)
            {
                Errors.Add($"Satır {lineNo}: {ColumnCount} sütun bekleniyordu, {c.Length} sütun bulundu.");
                continue;
            }

            // verileri olması gereken formata dönüştürür, kontrol eder
            if (!DateTime.TryParseExact(c[5], "dd.MM.yyyy", Tr, DateTimeStyles.None, out DateTime birthDate))
            {
                Errors.Add($"Satır {lineNo}: Doğum tarihi gg.aa.yyyy formatında olmalıdır.");
                continue;
            }

            if (!int.TryParse(c[8].Split('-')[0], out int academicYearStart))
            {
                Errors.Add($"Satır {lineNo}: Teslim yılı 2025-2026 formatında olmalıdır.");
                continue;
            }

            if (!int.TryParse(c[9], out int previousDays))
            {
                Errors.Add($"Satır {lineNo}: Önceki staj gün sayısı bir sayı olmalıdır.");
                continue;
            }

            // öğrenci daha önceden kaydedilmiş mi kontrol eder
            string studentNumber = c[4];
            if (existingNumbers.Contains(studentNumber))
            {
                Errors.Add($"Satır {lineNo}: {studentNumber} numaralı öğrenci zaten kayıtlı.");
                continue;
            }

            // yeni csvrecord nesnesi oluşturur
            var record = new CSVRecord
            {
                Name = c[0],
                Surname = c[1],
                NationalId = c[2],
                Nationality = c[3],
                StudentNumber = studentNumber,
                BirthDate = birthDate,
                Address = c[6],
                Department = c[7],
                AcademicYearStart = academicYearStart,
                PreviousInternshipDays = previousDays,
                Email = c[10],
                PhoneNumber = c[11],
                HealthAssistanceStatus = c[12]
            };

            _context.CSVRecords.Add(record);
            existingNumbers.Add(studentNumber);
            added++;
        }

        // hepsini veritabanına kaydeder
        await _context.SaveChangesAsync();

        if (added == 0 && Errors.Count == 0)
        {
            Message = "Dosyada veri satırı yok.";
            IsSuccess = false;
        }
        else
        {
            if (Errors.Count == 0)
            {
                Message = $"{added} kayıt başarıyla eklendi.";
            }
            else
            {
                Message = $"{added} kayıt eklendi, {Errors.Count} satır hatalı olduğu için atlandı.";
            }
            IsSuccess = Errors.Count == 0;
        }

        return Page();
    }
}