using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumberCalculator
{
    internal class PrimeTaskParams
    {
        public long minNum;
        public long maxNum;
        public List<long> primes = new List<long>();
        public float time;

        public PrimeTaskParams(long minNum, long maxNum)
        {
            this.minNum = minNum;
            this.maxNum = maxNum;
        }
    }
}
