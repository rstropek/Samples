interface IImageOptionsProtector
{
    string Protect(ImageOptions options);
    ImageOptions Unprotect(string data);
}

class ImageOptionsProtector(IDataProtectionProvider provider) : IImageOptionsProtector
{
    // In this program, we use the ASP.NET Data Protection API for encrypting and decrypting data.
    // Read more: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction
    // The following constant is the purpose string for the data protection API.
    // Read more: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/purpose-strings
    const string PROTECTION_PURPOSE = "BuildImageUrl";

    public string Protect(ImageOptions options)
    {
        byte[] bytes;
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                var validUntil = DateTimeOffset.Now.AddMinutes(1).Ticks;
                writer.Write(validUntil);
                writer.Write((byte)options);
            }
            bytes = stream.ToArray();
        }

        // Encrypt the byte array and base64 encode it
        var protector = provider.CreateProtector(PROTECTION_PURPOSE);
        var protectedBytes = protector.Protect(bytes);
        var protectedString = WebEncoders.Base64UrlEncode(protectedBytes);

        return protectedString;
    }

    public ImageOptions Unprotect(string data)
    {
        ImageOptions imageOptions;
        var protectedBytes = WebEncoders.Base64UrlDecode(data);
        var protector = provider.CreateProtector(PROTECTION_PURPOSE);
        var bytes = protector.Unprotect(protectedBytes);
        using var stream = new MemoryStream(bytes);
        using (var reader = new BinaryReader(stream))
        {
            var validUntil = reader.ReadInt64();
            if (validUntil < DateTimeOffset.Now.Ticks)
            {
                throw new InvalidImageOptionsException("Image URL has expired");
            }

            imageOptions = (ImageOptions)reader.ReadByte();
        }

        return imageOptions;
    }
}
