using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public interface IDataAccess
    {
        Task<string> GetDbNameAsync();
    }
}
