using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace StajEvrakSistemi.Pages.Student
{
    public class UploadModel : PageModel
    {
        [BindProperty] // formdan gönderilen bilgiyi bu özelliğe bağlar
        public IFormFile? UploadedFile { get; set; } // IFormFile yüklenen dosyayı temsil eden tür
        // UploadedFile değişkeni öğrencinin yüklediği dosyayı temsil ediyor

        public string Message { get; set; } = "";

        private readonly string[] allowedExtensions =
        {
            ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png"
        };

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            if (UploadedFile == null || UploadedFile.Length == 0) // öğrenci hiç dosya seçmemiş mi veya dosya var içeriği boş mu kontrolü
            {
                Message = "Lütfen bir dosya seçiniz.";
                return;
            }

            string extension = Path //string extension bulduğumuz uzantıyı saklar
                .GetExtension(UploadedFile.FileName)//UploadedFile.FileName dosyanın orijinal adı-Path.GetExtension dosyanın uzantısını alır
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension)) // !allowedExtensions.Contains(extension) ifadesi, uzantı izin verilen listede değilse demektir
            {
                Message = "Geçersiz dosya formatı. Lütfen DOC, DOCX, PDF, JPG, JPEG veya PNG yükleyiniz.";
                return;
            }

            string uploadFolder = Path.Combine( // Path.Combine(): Bu iki yolu işletim sistemine uygun biçimde birleştirir.
                Directory.GetCurrentDirectory(),  // Directory.GetCurrentDirectory() uygulamanın çalıştığı ana klasörün yolunu verir
                "UploadedDocuments" // "UploadedDocuments" dosyaları saklamak istediğimiz klasörün adı
            );

            Directory.CreateDirectory(uploadFolder);

            string safeFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadFolder, safeFileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(fileStream);
                }

                Message = "Dosyanız başarıyla yüklendi.";
            }
            catch (IOException)
            {
                Message = "Dosya kaydedilirken bir hata oluştu.";
            }
        }
    }
}