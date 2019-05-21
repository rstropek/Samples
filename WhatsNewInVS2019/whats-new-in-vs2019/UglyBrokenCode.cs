using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;

namespace WhatsNewInVS2019
{
    public class UglyBrokenCode{
        public UglyBrokenCode()
        {        }

        public int Add(int x, int y)
        {
            var z = x+y;
            return z;
        }

        public int GetRandom() {
            return new Random().Next(100);
        }

        public IEnumerable<int> GetManyRandoms() =>
        Enumerable.Range(0, 100).Select(_ => GetRandom());
    }
}
