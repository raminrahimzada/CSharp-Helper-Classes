using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HugeRational_ededler_by_Mathman
{
    class Program
    {
        static int M_quvvet(int a, int b)
        {
            Console.WriteLine("Vurulur:     " + a + " * " + b);
            if (b==0)
            {
                if (a!=0)
                {
                    return 1;
                }
                else
                {
                    throw new Exception("Xeta");
                }
            }
            if (b%2==0)
            {
                var t = M(a, b / 2);
                return t * t;
            }
            else
            {
                var t = M(a, (b-1) / 2);
                return t * t * a;
            }
        }
        static int M(int a, int b)
        {
            Console.WriteLine("loading...   " + a + "   *  " + b);
            if (b==0)
            {
                return 0;
            }
            if (b==1)
            {
                return a;
            }
            if (b%2==0)
            {
                var t = M(a, b / 2);
                return t + t;
            }
            else
            {
                var t = M(a, (b-1) / 2);
                return t + t + a;
            }
        }
        static double exp(double x)
        {
            int i = 1;

            double cvb = 1;
            double vuruq = 1;
            double cache = -1;
            while (Math.Abs(cache-cvb)!=0)
            {
                Console.WriteLine(cvb + " - " + cache);
                cache = cvb;
                vuruq *= x / i;//vur=x
                cvb += vuruq;//cvb=1+x
                i++;
            }
            return cvb;
        }
        static double ln(double eded)
        {
            double x = 1;
            double cache = -1;
            while(Math.Abs(cache-x)!=0)
            {
                //Console.WriteLine("x=" + x);
                cache = x;
                x = x - 1 + eded / Math.Exp(x);
                //Console.ReadKey();
            }
            return x;
        }
        static HyperFloat f(HyperFloat x)
        {
            return (x * x - 4 * x + 1) / (2 * x + 1);
        }
        static double F(double x)
        {
            return Math.Sqrt(1 + Math.Cos(x)) / (Math.Sin(x));
        }
        static void Main(string[] args)
        {

            Console.WriteLine(2*F(Math.PI + 0.0000001));
            return;
            HyperFloat.Dəqiqlik = 100;

            HyperFloat x = "-0.3";
            HyperFloat y = "5";
            Console.WriteLine((x / y).ToString());
            return;
            string limmit = f(2.0).ToString();
            Console.WriteLine("limit=" + limmit);
            //Console.ReadKey();

            return;
            Console.WriteLine("loading...\n");
            HyperFloat.Dəqiqlik = 78;
            Console.WriteLine(HyperFloat.EXP(HyperFloat.PI));
            Console.WriteLine(Math.Exp(Math.PI));
            
            return;

            Console.WriteLine("loading...");
            HyperFloat.Dəqiqlik = 75;
            HyperFloat eded="2";
            Console.WriteLine("\n\nln2="+HyperFloat.LN(eded,3).ToString());
            Console.WriteLine("ln2="+Math.Log(2.0));
            Console.WriteLine("by Mathman :D");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
/*
 * f(x)=exp(x)-z
 * f'(x)=exp(x)
 * 
 * t=t-(e^x-z)/e^x=t-1+z/e^x
 * 
 * 
 */
