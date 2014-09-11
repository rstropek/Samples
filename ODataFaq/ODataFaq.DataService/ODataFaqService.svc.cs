using ODataFaq.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;

namespace ODataFaq.DataService
{
	public class ODataFaqService : EntityFrameworkDataService<OrderManagementContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
			config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }
    }
}
