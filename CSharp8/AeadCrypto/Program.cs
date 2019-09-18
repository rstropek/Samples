using System.Threading.Tasks;

namespace AeadCrypto
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Aes.RegularAes();
            Aead.AesAead();
        }
    }
}
