using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PEuler
{
    class Problem2
    {
        public static void Main2(string[] args)
        {
            var fib = new List<BigInteger>() {1, 2};
            while (fib.Last()<4000000)
            {
                fib.Add(fib[fib.Count - 1] + fib[fib.Count - 2]);
            }
            Console.WriteLine(fib.Where(s => s%2 == 0).Select(s => (double) s).Sum());
        }
    }
}
