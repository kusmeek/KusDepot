namespace KusDepot.Reacts;

public static partial class StringSanitizer
{
    [GeneratedRegex("[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]",RegexOptions.Compiled)]
    public static partial Regex ControlCharsRegex();
    [GeneratedRegex("[\u202E\u202D\u202A-\u202C\u2066-\u2069]",RegexOptions.Compiled)]
    public static partial Regex DangerousUnicodeRegex();
    [GeneratedRegex(@"\s{20,}",RegexOptions.Compiled)]
    public static partial Regex ExcessiveWhitespaceRegex();
    [GeneratedRegex(@"[<>(){}\[\]/\\|]", RegexOptions.Compiled)]
    public static partial Regex ForbiddenPunctuationRegex();
    [GeneratedRegex(@"(?i)script",RegexOptions.Compiled)]
    public static partial Regex ScriptRegex();

    private const Int32 MaxLength = 200;

    public static String? Sanitize(String? input)
    {
        if(String.IsNullOrEmpty(input) || input.Length > MaxLength) { return null; }

        String normalized = input.IsNormalized(NormalizationForm.FormC) ? input : input.Normalize(NormalizationForm.FormC);

        if(
            ControlCharsRegex().IsMatch(normalized)         ||
            DangerousUnicodeRegex().IsMatch(normalized)     ||
            ForbiddenPunctuationRegex().IsMatch(normalized) ||
            ScriptRegex().IsMatch(normalized)               ||
            ExcessiveWhitespaceRegex().IsMatch(normalized)
        )
        {
            return null;
        }

        if(normalized.Contains('\0')) return null;

        return input;
    }
}