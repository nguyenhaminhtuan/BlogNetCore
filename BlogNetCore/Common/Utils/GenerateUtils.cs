using System.Text;

namespace BlogNetCore.Common.Utils;

public static class GenerateUtils
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static async Task<string> GenerateShortId()
    {
        return await Nanoid.Nanoid.GenerateAsync(Alphabet, 10);
    }

    public static string Slugify(string s)
    {
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(s); 
        var normalizeStr = Encoding.ASCII.GetString(bytes);
        return normalizeStr
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-");
    }
    
    public static string Slugify(string s, string postfix)
    {
        var slug = Slugify(s);
        return $"{slug}-{postfix}";
    }
    
}