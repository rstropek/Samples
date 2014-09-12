using ODataFaq.DataModel;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;

namespace ODataFaq.DataService
{
	public class ODataFaqService : EntityFrameworkDataService<OrderManagementContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
			config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
			config.SetEntitySetAccessRule("Customers", EntitySetRights.AllRead | EntitySetRights.WriteAppend);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }
    }
}
