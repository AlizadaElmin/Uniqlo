using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Uniqlo.Extensions;

public static class FileExtension
{
    public static bool IsValidType(this IFormFile file, string contentType)
    {
        return file.ContentType.StartsWith(contentType);
    }
    
    public static bool IsValidSize(this IFormFile file, long kb)
    {
        return file.Length <= kb * 1024;
    }

    public static async Task<string> UploadAsync(this IFormFile file, params string[] paths)
    {
        string uploadPath = Path.Combine(paths);
        if (!Path.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        string newFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        using (Stream fileStream = File.Create(Path.Combine(uploadPath, newFileName)))
        {
            await file.CopyToAsync(fileStream);
        }
        return newFileName;   
    }
}