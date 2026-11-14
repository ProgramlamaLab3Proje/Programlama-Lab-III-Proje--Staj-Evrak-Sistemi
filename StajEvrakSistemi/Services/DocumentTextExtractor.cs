// bu sınıfın görevi: dosya yolunu alıp uzantısına göre pdf veya docx okuyucusuna 
// çalıştırmak ve bulunan metni tek bir string olarak döndürmek
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace StajEvrakSistemi.Services;

public static class DocumentTextExtractor
{
    public static string ExtractText(string filePath)
    {
        string extension = Path.GetExtension(filePath)
            .ToLowerInvariant();

        return extension switch
        {
            ".pdf" => ExtractPdfText(filePath),
            ".docx" => ExtractDocxText(filePath),
            _ => throw new NotSupportedException(
                "Bu dosya türünden metin okunamıyor.")
        };
    }

    private static string ExtractPdfText(string filePath)
    {
        var text = new StringBuilder();

        using (var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }
        }

        return text.ToString();
    }

    private static string ExtractDocxText(string filePath)
    {
        var text = new StringBuilder();

        using (var document = WordprocessingDocument.Open(
            filePath, false))
        {
            var body = document.MainDocumentPart?.Document?.Body;
            if (body == null)
                return string.Empty;

            foreach (var paragraph in body.Descendants<
                DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                text.AppendLine(paragraph.InnerText);
            }
        }

        return text.ToString();
    }
}
