using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public interface IKeyVaultReader
    {
        bool IsAvailable { get; }
        Task<string> GetSecretAsync(string key);
    }
}
