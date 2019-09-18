using System;
using System.Security.Cryptography;
using System.Text;

namespace AeadCrypto
{
    public static class Aead
    {
        /// <summary>
        /// Encrypt/decrypt with Authenticated Encryption with Association Data (AEAD) algorithms
        /// </summary>
        public static void AesAead()
        {
            try
            {
                const string plaintext = "Here is some data to encrypt!";

                // Generate random key and iv (aka nonce)
                var key = new byte[16];
                RandomNumberGenerator.Fill(key);
                var iv = new byte[12];
                RandomNumberGenerator.Fill(iv);

                // Encrypt the string to an array of bytes.
                // Note that we provide "associated data". The ciphertext can only be decrypted
                // when given the same "associated data". Also note that we do not only get 
                // ciphertext but also a "Message authentication code" (MAC, aka "tag"). We can
                // use this MAC to confirm that the ciphertext has not been changed.
                var (ciphertext, mac) = EncryptStringToBytesAsync(plaintext, "1", key, iv);

                // Experiment: Tamper with some bytes in `result.Ciphertext` and see error

                // Decrypt the bytes to a string.
                // Experiment: Pass different "associated data" and see error
                string roundtrip = DecryptStringFromBytesAsync(ciphertext, "1", mac, key, iv);

                //Display the original data and the decrypted data.
                Console.WriteLine($"Original:\t{plaintext}");
                Console.WriteLine($"Round Trip:\t{roundtrip}");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
            }
        }

        static (byte[] ciphertext, byte[] mac) EncryptStringToBytesAsync(string plainText, string context, byte[] key, byte[] iv)
        {
            var dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            var associatedData = Encoding.UTF8.GetBytes(context);
            var mac = new byte[16];
            var encrypted = new byte[dataToEncrypt.Length];
            using (var aes = new AesGcm(key))
            {
                aes.Encrypt(iv, dataToEncrypt, encrypted, mac, associatedData);
            }

            return (encrypted, mac);
        }

        static string DecryptStringFromBytesAsync(byte[] ciphertext, string context, byte[] mac, byte[] key, byte[] iv)
        {
            var decrypted = new byte[ciphertext.Length];
            var associatedData = Encoding.UTF8.GetBytes(context);
            using (var aes = new AesGcm(key))
            {
                aes.Decrypt(iv, ciphertext, mac, decrypted, associatedData);
            }

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
