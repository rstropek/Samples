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

namespace ExtensionEverything
{
    using MultipleStringExtensions;

    public static class EmailAddressExtensions
    {
        extension(string emailAddress)
        {
            // Extension property
            public bool IsEmail => emailAddress.IsValidEmail();
        }

        extension(string someNumberAsString)
        {
            // Extension operator
            // (Note: Compound assignment operators, also new in C# 14)
            public void operator +=(string v)
            {
                if (int.TryParse(someNumberAsString, out var left) && int.TryParse(v, out var right))
                {
                    someNumberAsString = (left + right).ToString();
                }
                else
                {
                    throw new InvalidOperationException("Both operands must be valid integers in string format.");
                }
            }
        }
    }
}
