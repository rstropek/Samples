using Microsoft.EntityFrameworkCore;

namespace Data;

public static class SeedExtensions
{
    extension(ApplicationDataContext db)
    {
        public async Task SeedOrdersIfEmptyAsync(
            int total = 100_000,
            int batchSize = 5_000,
            CancellationToken ct = default)
        {
            if (await db.Orders.AsNoTracking().AnyAsync(ct)) return;

            var rnd = new Random();
            var now = DateTime.UtcNow;
            var startDate = new DateTime(2023, 1, 1);

            var countries = new[] { "DE", "AT", "CH", "NL", "FR", "IT", "ES", "SE", "PL" };
            var citiesPerCountry = new Dictionary<string, string[]>
            {
                ["DE"] = ["Berlin", "Hamburg", "München", "Köln", "Frankfurt"],
                ["AT"] = ["Wien", "Graz", "Linz", "Salzburg", "Innsbruck"],
                ["CH"] = ["Zürich", "Genf", "Basel", "Bern", "Lausanne"],
                ["NL"] = ["Amsterdam", "Rotterdam", "Utrecht", "Eindhoven"],
                ["FR"] = ["Paris", "Lyon", "Marseille", "Toulouse"],
                ["IT"] = ["Mailand", "Rom", "Turin", "Neapel"],
                ["ES"] = ["Madrid", "Barcelona", "Valencia", "Sevilla"],
                ["SE"] = ["Stockholm", "Göteborg", "Malmö"],
                ["PL"] = ["Warschau", "Krakau", "Danzig", "Posen"],
            };
            const string currency = "EUR";

            var oldAutoDetect = db.ChangeTracker.AutoDetectChangesEnabled;
            db.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                var counter = 0;
                while (counter < total)
                {
                    var take = Math.Min(batchSize, total - counter);
                    var batch = new List<Order>(take);

                    for (var i = 0; i < take; i++)
                    {
                        var country = countries[rnd.Next(countries.Length)];
                        var cityList = citiesPerCountry[country];
                        var city = cityList[rnd.Next(cityList.Length)];

                        var orderDate = RandomDate(rnd, startDate, now);
                        var amount = RandomAmount(rnd);

                        var status = WeightedChoice(rnd,
                            ("Paid", 0.55), ("Pending", 0.20), ("Shipped", 0.20), ("Cancelled", 0.05));

                        var orderNumber = $"ORD-{orderDate:yyyy}-{counter + i + 1:000000}";

                        var order = new Order
                        {
                            OrderNumber = orderNumber,
                            OrderDate = orderDate,
                            Amount = amount,
                            Currency = currency,
                            Country = country,
                            City = city,
                            Status = status,
                        };

                        batch.Add(order);
                    }

                    await db.Orders.AddRangeAsync(batch, ct);
                    await db.SaveChangesAsync(ct);

                    counter += take;
                    db.ChangeTracker.Clear();
                }
            }
            finally
            {
                db.ChangeTracker.AutoDetectChangesEnabled = oldAutoDetect;
            }
        }
    }

    private static DateTime RandomDate(Random rnd, DateTime from, DateTime to)
    {
        var range = (to - from).TotalSeconds;
        var offset = rnd.NextDouble() * range;
        return from.AddSeconds(offset);
    }

    private static decimal RandomAmount(Random rnd)
    {
        var u1 = 1.0 - rnd.NextDouble();
        var u2 = 1.0 - rnd.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        var mean = Math.Log(150);
        const double sigma = 0.9;
        var logNormal = Math.Exp(mean + sigma * randStdNormal);

        var value = Math.Clamp(logNormal, 5.0, 2_000.0);
        return Math.Round((decimal)value, 2);
    }

    private static T WeightedChoice<T>(Random rnd, params (T item, double weight)[] items)
    {
        var sum = items.Sum(x => x.weight);
        var pick = rnd.NextDouble() * sum;
        double acc = 0;
        foreach (var (item, weight) in items)
        {
            acc += weight;
            if (pick <= acc) return item;
        }
        return items[^1].item;
    }
}