using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public interface IKeyVaultReader
    {
        Task<string> GetSecretAsync(string key);
    }
}
