using System;
using System.Text;

class Rum
    {
        const string Yuz = "C";
        const string Min = "M";
        const string On = "X";
        const string Bes_Yuz = "D";
        const string Elli = "L";
        const string acilis = "(";
        const string baglanis = ")";

        static string Minqat(ulong i)
        {
            if (Cevir(i) != "") return (acilis + Cevir(i) + baglanis);
            else return ("");

        }
        static string Teklik(ulong i)
        {
            switch (i)
            {
                case 1: return "I";
                case 2: return "II";
                case 3: return "III";
                case 4: return "IV";
                case 5: return "V";
                case 6: return "VI";
                case 7: return "VII";
                case 8: return "VIII";
                case 9: return ("I" + On);
                default: return "";
            }
        }
        static string Yuzluk(ulong i)
        {
            switch (i)
            {
                case 1: return Yuz;
                case 2: return Yuz + Yuz;
                case 3: return Yuz + Yuz + Yuz;
                case 4: return Yuz + Bes_Yuz;
                case 5: return Bes_Yuz;
                case 6: return Bes_Yuz + Yuz;
                case 7: return Bes_Yuz + Yuz + Yuz;
                case 8: return Bes_Yuz + Yuz + Yuz + Yuz;
                case 9: return Yuz + Min;
                default: return "";
            }
        }
        static string Onluqlar(ulong i)
        {
            switch (i)
            {
                case 1: return On;
                case 2: return On + On;
                case 3: return On + On + On;
                case 4: return On + Elli;
                case 5: return Elli;
                case 6: return Elli + On;
                case 7: return Elli + On + On;
                case 8: return Elli + On + On + On;
                case 9: return On + Yuz;
                default: return "";
            }
        }
        static string BesYuzlukler(ulong i)
        {
            string s = "";
            for (ulong j = 0; j < (i / 500); j++)
            {
                s += Bes_Yuz[0];
            }
            return s;
        }
        public static string Cevir(ulong x)
        {
            //Console.WriteLine("Cevir icinde x=" + x);
            //Console.ReadLine();
            if (x == 0) { return ""; }
            string cavab = "";
            //cavab += Minqat(x / 1000);
            x = x - 1000 * (x / 1000);
            string w = "";
            for (ulong y = 0; y < (x / 500); y++)
            {
                w += Bes_Yuz;
            }
            cavab += w;
            x = x - 500 * (x / 500);
            cavab += Yuzluk(x / 100);
            x = x - 100 * (x / 100);
            cavab += Onluqlar(x / 10);
            x = x - 10 * (x / 10);
            cavab += Teklik(x);
            return cavab;
        }
        public static ulong Ters_Cevir(string s)
        {
            //Console.WriteLine("ters cevir:  " + s);
            for (ulong i = 1; i < 1000000000; i++)
            {
              //  Console.WriteLine("loading..."+i);
                if (Cevir(i)==s)
                {
                    return i;
                }
            }
            Console.WriteLine("Tapmadi: "+s);
            return 0;
        }
    }
    
    class Program
    {
        const char Yuz = 'C';
        const char Min = 'M';
        const char On = 'X';
        const char Bes_Yuz = 'D';
        const char Bes = 'V';
        const char Elli = 'L';

        static string M(int x)
        {
            if (x>=1000)
            {
                return new string(Min, (x / 1000)) + M(x % 1000);
            }
            if (x>=500)
            {
                return new string(Bes_Yuz, (x / 500)) + M(x % 500);
            }
            if (x >=100)
            {
                return new string(Yuz, (x / 100)) + M(x % 100);
            }
            if (x >=50)
            {
                return new string(Elli, (x / 50)) + M(x % 50);
            }
            if (x >=10)
            {
                return new string(On, (x / 10)) + M(x % 10);
            }
            if (x >=5)
            {
                return new string(Bes, (x / 5)) + M(x % 5);
            }
            return new string('I', x);
        }
        static int TERS_M(string eded)
        {
            for (int i = 0; i < 10000; i++)
            {
                if (M(i)==eded)
                {
                    return i;
                }
            }
            throw new Exception();
            return -1;
        }
        static void Main(string[] args)
        { 
            string s = Console.ReadLine();
            string s1 = s.Split('+')[0];
            string s2 = s.Split('+')[1];
            var ts1 = Rum.Ters_Cevir(s1);
            var ts2 = Rum.Ters_Cevir(s2);
            //Console.WriteLine("ts1=" + ts1);
            //Console.WriteLine("ts2=" + ts2);
            Console.WriteLine(Rum.Cevir(ts1 + ts2));
        }
    }
