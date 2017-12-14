using System;
using System.Collections.Generic;
using bint = System.Numerics.BigInteger;

namespace ECM
{
    public class Program
    {
        static Tuple<bint, bint>[] pow2store; //caches the values of 2^n * P
        static bint Factor;
        static bint max2powstored = 0;
        static int maxexp = 0;

        public static void Main(string[] args)
        {
            
            pow2store = new Tuple<bint, bint>[100000];
            bint n = 1111111111111111111;
            //11111111111111111 = 2071723 * 5363222357;
            //curve params from wiki article
            bint x = 1;
            bint y = 1;
            bint a = 5;
            bint b = (y * y - x * x * x - a * x) % n;
            bool ftest = false;
            var P = new Tuple<bint, bint>(x, y);
            pow2store[0] = P;
            var twop = twoP(b, P, n, out ftest);
            pow2store[1] = twop;
            int factsteps = 1;
            bint factorial = 1;
            while (!ftest)
            {
                factorial *= ++factsteps;
                Console.WriteLine("calculating {0}! p", factsteps);
                CalcNP(factorial, b, n, out ftest);
            }
            Console.WriteLine("{0} = {1} * {2}", n, Factor, n / Factor);
            Console.ReadKey(true);
        }

        static Tuple<bint, bint> CalcNP(bint calc, bint b, bint n, out bool res)
        {
            int powguess = (int)Math.Floor(bint.Log(calc, 2));
            powguess = Math.Min(powguess, maxexp);
            bint max2pow = bint.Pow(2, (int)powguess);
            while (max2pow * 2 <= calc)
            {
                max2pow *= 2;
                powguess++;
                if (max2pow > max2powstored)
                {
                    maxexp++;
                    max2powstored = max2pow;
                    pow2store[powguess] = twoP(b, pow2store[powguess - 1], n, out res);
                    if (res)
                    {
                        return pow2store[powguess];
                    }
                }
            }
            calc -= max2pow;
            if (calc > 1)
            {
                var Q = CalcNP(calc, b, n, out res);
                if (res)
                {
                    return new Tuple<bint, bint>(0, 0);
                }
                return ECadd(pow2store[powguess], Q, n, out res);
            }
            else
            {
                res = false;
                return pow2store[powguess];
            }
        }

        static Tuple<bint, bint> twoP(bint b, Tuple<bint, bint> P, bint n, out bool Factor)
        {
            bint stop = (3 * P.Item1 * P.Item1 - b) % n;
            bint sbottom = (2 * P.Item2) % n;
            bint inv = ModInv(sbottom, n, out Factor);
            if (Factor)
            {
                return new Tuple<bint, bint>(0, 0);
            }
            bint s = (stop * inv) % n;
            bint xR = (s * s - 2 * P.Item1) % n;
            bint yR = (s * (P.Item1 - xR) - P.Item2) % n;
            return new Tuple<bint, bint>(xR, yR);
        }

        static Tuple<bint, bint> ECadd(Tuple<bint, bint> P, Tuple<bint, bint> Q, bint n, out bool Factor)
        {
            bint stop = P.Item2 - Q.Item2 % n;
            bint sbottom = (P.Item1 - Q.Item1) % n;
            bint inv = ModInv(sbottom, n, out Factor);
            if (Factor)
            {
                return new Tuple<bint, bint>(0, 0);
            }
            bint s = (stop * inv) % n;
            bint xR = (s * s - P.Item1 - Q.Item1) % n;
            bint yR = (s * (xR - P.Item1) - P.Item2) % n;
            return new Tuple<bint, bint>(xR, yR);
        }

        static bint ModInv(bint a, bint m, out bool notcoprime)
        {
            bint[] arr = ExtGCD(a, m);
            if (!bint.Abs(arr[2]).IsOne)
            {
                Console.WriteLine("found factor when inverting {0} mod {1}", (a + m) % m, m);
                Factor = arr[2];
                notcoprime = true;
                return 0;
            }
            else
            {
                notcoprime = false;
                return arr[0];
            }
        }

        /// <summary>
        /// Bu metod verilmis a ve b ededleri ucun au+bv=1 tenlinin helli olan
        /// u ve v ededlerini hemcinin eger emsallarin ortaq boleni olarsa onda hemin 
        /// bolene bolub =1 syazib hell eliyir ve o dediyim ixtisar emsalini da ucuncu element olaraq qaytarir
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static bint[] ExtGCD(bint a, bint b)
        {
            bint x = 0;
            bint y = 1;
            bint u = 1;
            bint v = 0;
            while (b != 0)
            {
                //Console.WriteLine(a + "-" + b + "-" + " ; " + u + "-" + v);
                bint buffer = b;
                bint q = a / b;
                b = a % b;
                a = buffer;
                buffer = x;
                x = u - q * x;
                u = buffer;
                buffer = y;
                y = v - q * y;
                v = buffer;
            }
            return new bint[] { u, v, a };

        }
    }
}
