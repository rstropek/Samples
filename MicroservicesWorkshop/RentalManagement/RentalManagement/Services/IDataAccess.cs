using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public interface IDataAccess
    {
        Task<string> GetDbNameAsync();
        Task<string> GetConnectionString();
    }
}
