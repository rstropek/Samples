namespace StringExtensionsTraditional;

public static class StringExtensionsTraditional
{
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email)) { return false; }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch { return false; }
    }

    public static string TruncateWithSuffix(this string str, int maxLength, string suffix = "...")
    {
        if (str.Length <= maxLength) { return str; }
        return string.Concat(str.AsSpan(0, maxLength - suffix.Length), suffix);
    }
}
