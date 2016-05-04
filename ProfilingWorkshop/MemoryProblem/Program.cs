using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryProblem
{
    class Product
    {
        public byte[] Image { get; set; }

        public int Price { get; set; }

        public Product[] SubProducts { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var productCache = new Queue<Product>();

            // Fill cache with 100 products
            for (var i = 0; i < 100; i++)
            {
                productCache.Enqueue(GenerateDemoProduct());
            }

            for (var i = 0; i < 1000; i++)
            {
                // Calculate the average price of products in cache
                var averagePrice = productCache.Average(p => p.Price);
                Console.WriteLine(averagePrice);

                // Remove oldest 10 products, add 10 new products
                for (var j = 0; j < 10; j++)
                {
                    productCache.Dequeue();
                    productCache.Enqueue(GenerateDemoProduct());
                }
            }
        }

        private static int seed = 0;

        public static Product GenerateDemoProduct()
        {
            var random = new Random(++seed);
            var result = new Product
            {
                Image = new byte[random.Next(1024, 10240)],
                Price = random.Next(10, 20)
            };

            if (random.Next(0, 3) == 0)
            {
                var numberOfSubProducts = random.Next(0, 6);
                result.SubProducts = new Product[numberOfSubProducts];
                for (var i = 0; i < numberOfSubProducts; i++)
                {
                    result.SubProducts[i] = GenerateDemoProduct();
                }
            }

            return result;
        }
    }
}
