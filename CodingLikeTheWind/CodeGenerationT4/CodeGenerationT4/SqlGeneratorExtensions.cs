using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Property
    {
        public string Code { get; set; }
        public string DBDataType { get; set; }
    }

    public partial class SqlGenerator
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public List<Property> Properties { get; set; }
    }
}
