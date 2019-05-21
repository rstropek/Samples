using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatsNewInVS2019
{
    public class Debugging
    {
        public static void DoSomethingWithTickets()
        {
            var fullTicketInfo = SampleData.Tickets
                .Select(t =>
                {
                    var connection = SampleData.Connections.First(c => c.From == t.From && c.To == t.To);
                    return new
                    {
                        t.ID,
                        Connection = new
                        {
                            From = SampleData.Bases.First(b => b.Name == connection.From),
                            To = SampleData.Bases.First(b => b.Name == connection.To),
                            connection.Distance,
                            connection.Price
                        },
                        t.Passenger,
                        t.Price
                    };
                }).ToArray();
        }

        private bool somethingImportantHappened = false;

        public void LetSomethingHappen()
        {
            somethingImportantHappened = !somethingImportantHappened;
        }
    }

    public class Base
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class Ticket
    {
        public DateTime Date { get; set; }

        public string ID { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal Price { get; set; }

        public string Passenger { get; set; }

        public TimeRelation Time => Date >= DateTime.Today ? TimeRelation.Upcoming : TimeRelation.Past;
    }

    public enum TimeRelation
    {
        Past,
        Upcoming
    }

    public class Connection : IEquatable<Connection>
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Distance { get; set; }
        public decimal Price { get; set; }

        public bool Equals(Connection other) =>
            From == other.From && To == other.To && Distance == other.Distance && Price == other.Price;
    }

    public static class SampleData
    {
        public static Base[] Bases { get; } = new[]
        {
            new Base { Name = "Cybertron", Image = "../images/cybertron.jpg" },
            new Base { Name = "Earth", Image = "../images/earth.jpg" },
            new Base { Name = "Krypton", Image = "../images/krypton.jpg" },
            new Base { Name = "Pandora", Image = "../images/pandora.jpg" },
            new Base { Name = "Arrakis", Image = "../images/arrakis.jpg" },
            new Base { Name = "Tatooine", Image = "../images/tatooine.jpg" },
            new Base { Name = "Vulcan", Image="../images/vulcan.png" },
            new Base { Name = "Decapod 10", Image="../images/decapod-10.jpg" },
            new Base { Name = "Magrathea", Image="../images/magrathea.png" }

        };

        public static Ticket[] Tickets { get; } = new[]
        {
            new Ticket { ID = "XE34AV6", From = "Cybertron", To = "Earth", Passenger = "Optimus Prime", Price = 499.99m, Date = new DateTime(2007, 7, 4) },
            new Ticket { ID = "U9XZLR9", From = "Cybertron", To = "Earth", Passenger = "Bumblebee", Price = 349.99m, Date = new DateTime(2005, 8, 20) },
            new Ticket { ID = "K40JS7V", From = "Krypton", To = "Earth", Passenger = "Kal-El", Price = 139.99m, Date = new DateTime(1947, 3, 20) },
            new Ticket { ID = "ACXC43O", From = "Earth", To = "Pandora", Passenger = "Jake Sully", Price = 89.99m, Date = new DateTime(2154, 10, 8) },
            new Ticket { ID = "ZA34SD0", From = "Decapod 10", To = "Earth", Passenger = "Dr. John Zoidberg", Price = 1999.99m, Date = new DateTime(2929, 1, 15) },
            new Ticket { ID = "M424242", From = "Earth", To = "Magrathea", Passenger = "Arthur Dent", Price = 4242.42m, Date = new DateTime(2042, 1, 31) }
        };

        public static Connection[] Connections { get; } = new[]
        {
            new Connection { From = "Decapod 10", To = "Earth", Distance = 1000, Price = 100m },
            new Connection { From = "Cybertron", To = "Earth", Distance = 100, Price = 375.75m },
            new Connection { From = "Krypton", To = "Earth", Distance = 12300, Price = 7499.99m },
            new Connection { From = "Tatooine", To = "Vulcan", Distance = 5200, Price = 89.90m },
            new Connection { From = "Earth", To = "Pandora", Distance = 3750, Price = 301.01m },
            new Connection { From = "Earth", To = "Magrathea", Distance = 4242, Price = 4242.42m },
            new Connection { From = "Arrakis", To = "Decapod 10", Distance = 7300, Price = 1349.90m },
            new Connection { From = "Arrakis", To = "Pandora", Distance = 7300, Price = 1349.90m },
            new Connection { From = "Pandora", To = "Magrathea", Distance = 4242, Price = 4242.42m },
            new Connection { From = "Tatooine", To = "Krypton", Distance = 1234, Price = 1345.67m },
        };
    }
}
