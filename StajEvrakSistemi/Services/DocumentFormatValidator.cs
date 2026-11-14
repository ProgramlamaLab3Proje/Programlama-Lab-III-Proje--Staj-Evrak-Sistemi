
using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace StajEvrakSistemi.Services;

public static class DocumentFormatValidator
{
    public static bool IsValid(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        string extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        using var stream = file.OpenReadStream();

        try
        {
            return extension switch
            {
                ".pdf" => HasSignature(stream,
                    new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }),

                ".jpg" or ".jpeg" => HasSignature(stream,
                    new byte[] { 0xFF, 0xD8, 0xFF }),

                ".png" => HasSignature(stream,
                    new byte[] {
                        0x89, 0x50, 0x4E, 0x47,
                        0x0D, 0x0A, 0x1A, 0x0A
                    }),

                ".doc" => HasSignature(stream,
                    new byte[] {
                        0xD0, 0xCF, 0x11, 0xE0,
                        0xA1, 0xB1, 0x1A, 0xE1
                    }),

                ".docx" => IsValidDocx(stream),

                _ => false
            };
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is InvalidDataException ||
            ex is ArgumentException)
        {
            return false;
        }
    }

    private static bool HasSignature(Stream stream, byte[] signature)
    {
        byte[] buffer = new byte[signature.Length];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        return bytesRead == signature.Length &&
               buffer.SequenceEqual(signature);
    }

    private static bool IsValidDocx(Stream stream)
    {
        using var archive = new ZipArchive(
            stream,
            ZipArchiveMode.Read
        );

        bool hasContentTypes = archive.GetEntry("[Content_Types].xml") != null;
        bool hasDocument = archive.GetEntry("word/document.xml") != null;

        return hasContentTypes && hasDocument;
    }
}
