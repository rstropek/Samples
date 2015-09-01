using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingLikeTheWind.Data
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }

        public double Price
        {
            get { return Amount * UnitPrice; }
        }
    }
}
