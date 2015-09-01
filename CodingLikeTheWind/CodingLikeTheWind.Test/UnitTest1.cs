using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingLikeTheWind.Data;
using System.Collections.Generic;
using System.Linq;

namespace CodingLikeTheWind.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var manager = new DataManager();

            var amountBefore = manager.GetTotalAmount();
            manager.InsertOrder("Testbestellung", 10, 13);
            var amountAfter = manager.GetTotalAmount();

            Assert.AreEqual(amountAfter - amountBefore, 130);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var manager = new DataManager();

            var rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                var amount = rand.Next(100);
                var unitPrice = rand.NextDouble() * 1000;

                manager.InsertOrder("Test " + i, amount, unitPrice);
            }

            IList<Order> orders = manager.GetOrders();
            var specificOrder = orders.FirstOrDefault(p => p.OrderId == 27);
        }

    }
}
