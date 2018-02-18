using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentalManagement.Services;
using System;

namespace RentalManagement.Test
{
    [TestClass]
    public class RatingTest
    {
        [TestMethod]
        public void TestFreeShortTrip()
        {
            var rating = new Rating();
            Assert.AreEqual(0m, rating.CalculateTotalCosts(TimeSpan.FromMinutes(10d)));
        }

        [TestMethod]
        public void TestFirstHourFlatFeeTrip()
        {
            var rating = new Rating();
            Assert.AreEqual(3m, rating.CalculateTotalCosts(TimeSpan.FromMinutes(45d)));
        }

        [TestMethod]
        public void TestLongTrip()
        {
            var rating = new Rating();
            Assert.AreEqual(13m, rating.CalculateTotalCosts(TimeSpan.FromMinutes(130d)));
        }
    }
}
