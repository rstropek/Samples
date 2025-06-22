namespace StaticExtensions;

static class BatmanGenerator
{
    extension(string)
    {
        public static string GenerateBatman(int numberOfNas)
        {
            const string batman = "Batman!";

            // Generates something like "NaNaNaNaNaNaNaNaNaNaNaNaNaNaNaNa Batman!"
            // See https://youtu.be/kK4H-LkrQjQ
            if (numberOfNas < 1)
            {
                return batman;
            }

            // Each "Na" is 2 characters, so total Na characters = numberOfNas * 2
            // Total length = (numberOfNas * 2) + 1 space + batman.Length
            return String.Create(numberOfNas * 2 + batman.Length + 1, numberOfNas, (span, numberOfNas) =>
            {
                // Generate the "Na" pairs
                for (int i = 0; i < numberOfNas; i++)
                {
                    span[i * 2] = 'N';
                    span[i * 2 + 1] = 'a';
                }

                // Add space between Nas and Batman
                span[numberOfNas * 2] = ' ';

                // Copy Batman to the end
                batman.AsSpan().CopyTo(span[(numberOfNas * 2 + 1)..]);
            });
        }
    }
}