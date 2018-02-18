using RentalManagement.Model;
using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public interface IServiceBusSender
    {
        Task SendRentalEndResult(RentalEndResult result);
    }
}
