using System;
using System.Text;
using System.Threading;

namespace AspNetCore1Angular2Intro.Services
{
    public class NameGenerator : INameGenerator
    {
        #region Demo data
        private readonly string[][] parts = new[]
        {
            new [] { "The", "A", "One Hell of a", "Yesterday's", "Rising of", "In Love With" },
            new [] { "Dark", "Light", "Cruel", "Funny", "Beatiful", "Charming" },
            new [] { "Street", "Road", "Guy", "Knight", "Women", "Man" }
        };
        #endregion

        private static int seed = 0;

        public string GenerateRandomBookTitle()
        {
            var rand = new Random(Interlocked.Increment(ref seed));
            var result = new StringBuilder();
            foreach (var part in parts)
            {
                if (result.Length > 0)
                {
                    result.Append(' ');
                }

                result.Append(part[rand.Next(part.Length)]);
            }

            return result.ToString();
        }
    }
}
