using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Uniqlo.Extensions;

public static class FileExtension
{
    public static bool IsValidType(this string contentType)
    {
        return contentType.StartsWith("image/");
        
    }

    public static bool IsValidSize(this long kb)
    {
        return kb <= 2048;
    }

    public static string Upload(this string path)
    {
        return Path.GetRandomFileName() + Path.GetExtension(path);
    }
}