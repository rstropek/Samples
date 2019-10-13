using System.Collections.Generic;

namespace GrpcServer.Services
{
    public class MathAlgorithms
    {
        public IEnumerable<int> GetFibonacci()
        {
            var previous = 0;
            var current = 1;
            while (true)
            {
                (previous, current) = (current, previous + current);
                yield return current;
            }
        }
    }
}
