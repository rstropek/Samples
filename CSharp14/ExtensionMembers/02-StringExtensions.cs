namespace StringExtensions
{
    public static class StringExtensions
    {
        extension(string stringToValidate)
        {
            public bool IsValidEmail()
            {
                if (string.IsNullOrWhiteSpace(stringToValidate)) { return false; }

                try
                {
                    var addr = new System.Net.Mail.MailAddress(stringToValidate);
                    return addr.Address == stringToValidate;
                }
                catch { return false; }
            }

            public string TruncateWithSuffix(int maxLength, string suffix = "...")
            {
                if (stringToValidate.Length <= maxLength) { return stringToValidate; }
                return string.Concat(stringToValidate.AsSpan(0, maxLength - suffix.Length), suffix);
            }
        }
    }
}

namespace MultipleStringExtensions
{
    public static class StringMultipleExtensions
    {
        extension(string emailAddress)
        {
            // Note that we cannot declare fields in extension blocks

            public bool IsValidEmail()
            {
                if (string.IsNullOrWhiteSpace(emailAddress)) { return false; }

                try
                {
                    var addr = new System.Net.Mail.MailAddress(emailAddress);
                    return addr.Address == emailAddress;
                }
                catch { return false; }
            }
        }

        extension(string longString)
        {
            public string TruncateWithSuffix(int maxLength, string suffix = "...")
            {
                if (longString.Length <= maxLength) { return longString; }
                return string.Concat(longString.AsSpan(0, maxLength - suffix.Length), suffix);
            }
        }
    }
}

namespace ExtensionProperties
{
    using MultipleStringExtensions;

    public static class EmailAddressExtensions
    {
        extension(string emailAddress)
        {
            public bool IsEmail => emailAddress.IsValidEmail();
        }
    }
}
