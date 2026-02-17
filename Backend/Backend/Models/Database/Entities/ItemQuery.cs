using System.Globalization;
using System.Text;

namespace Backend.Models.Database.Entities;

public class ItemQuery
{
    private string? _search;
    public string? Search
    {
        get => _search;
        set => _search = NormalizeString(value);
    }

    public string? Sort { get; set; } = "asc";

    public static string? NormalizeString(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        var normalizedString = text.Trim().ToLower().Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }    
}
