using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

// Learn more about Using Declarations at https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#using-declarations

namespace UsingDeclarations
{
    class Program
    {
        static async Task Main()
        {
            const string plaintext = "Here is some data to encrypt!";

            // The old syntax would have been:
            // using (var aes = new AesManaged()) { ... }
            using var aes = new AesManaged();

            // Encrypt the string to an array of bytes.
            byte[] ciphertext = await EncryptStringToBytesAsync(plaintext, aes.Key, aes.IV);

            // Decrypt the bytes to a string.
            string roundtrip = await DecryptStringFromBytesAsync(ciphertext, aes.Key, aes.IV);

            //Display the original data and the decrypted data.
            Console.WriteLine($"Original:\t{plaintext}");
            Console.WriteLine($"Round Trip:\t{roundtrip}");
        }

        static async Task<byte[]> EncryptStringToBytes_OldSyntax_Async(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (var aes = new AesManaged())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                await swEncrypt.WriteAsync(plainText);
                            }

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
            }

            return encrypted;
        }

        static async Task<byte[]> EncryptStringToBytesAsync(string plainText, byte[] Key, byte[] IV)
        {
            using var aes = new AesManaged { Key = Key, IV = IV };
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);
            await swEncrypt.WriteAsync(plainText);
            return msEncrypt.ToArray();
        }

        static async Task<string> DecryptStringFromBytesAsync(byte[] cipherText, byte[] Key, byte[] IV)
        {
            using var aes = new AesManaged { Key = Key, IV = IV };
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return await srDecrypt.ReadToEndAsync();
        }
    }
}
