using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;

namespace Qaus_usulu
{
    internal static class HelpSuperNumber
    {
        public static SuperNumber ToSuperNumber(this string s)
        {
            return new SuperNumber(s);
        }
    }
    public class SuperNumber
    {
        #region private variables
        private BigInteger _suret;
        private BigInteger _mexrec;
        #endregion

        #region public Static variables 
        public static char OnluqSimvol = '.';
        public static CultureInfo DefaultCultureInfo = CultureInfo.InvariantCulture;
        public static int Deqiqlik = 10;
        public static int IterationCount = 100;
        #endregion

        #region consts
        private const string EStr = "2.71828182845904590";
        private const string PiStr = "3.1415926535897932384626433832795028841971693993751058";
        #endregion

        #region static fields
        private static SuperNumber E
        {
            get
            {
                return EStr
                    //.Substring(0, Deqiqlik)
                    .ToSuperNumber();
            }
        }
        private static SuperNumber PI
        {
            get
            {
                return PiStr
                    //.Substring(0, Deqiqlik)
                    .ToSuperNumber();
            }
        }
        public static SuperNumber Epsilon
        {
            get { return new SuperNumber("0.0" + new string('0', Deqiqlik - 3) + "1"); }
        }
        #endregion

        #region ctors
        // "1.25"
        public SuperNumber(BigInteger suret, BigInteger mexrec)
        {
            _suret = suret;
            _mexrec = mexrec;
        }

        public SuperNumber(BigInteger number)
        {
            _suret = number;
            _mexrec = 1;
        }
        public SuperNumber(string numberStr)
        {
            // tam ededdirse
            if (!numberStr.Contains(OnluqSimvol))
            {
                _suret = BigInteger.Parse(numberStr);
                _mexrec = 1;
            }
            else
            {
                // 1.25
                var numberwithoutOnluq = numberStr.Replace(OnluqSimvol.ToString(DefaultCultureInfo), "");//125
                var onluqindex = numberStr.IndexOf(OnluqSimvol);//1
                var fulllength = numberStr.Length;//3
                var mertebesayi = fulllength - onluqindex;//2
                _suret = BigInteger.Parse(numberwithoutOnluq);//125
                _mexrec = BigInteger.Parse("1" + new string('0', mertebesayi-1));//100    
            }
        }
        #endregion

        #region Fields

        public BigInteger Suret
        {
            get { return _suret; }
        }

        public BigInteger Mexrec
        {
            get { return _mexrec; }
        }

        public   string ToStringRational()
        {
            return Suret + "/" + Mexrec;
        }

        public BigInteger TamHisse()
        {
            string fullnumber = this.ToString();
            if (fullnumber.Contains(OnluqSimvol))
            {
                return BigInteger.Parse(fullnumber);
            }
            else
            {
                int index = fullnumber.IndexOf(OnluqSimvol);
            }
            return -1;
        }
        #endregion

        #region help methods
 public void IxtisarEt()
        {
            if (Suret==0)
            {
                return;
            }
            var t = Ebob(_mexrec, _suret);
            if (t == 1) return;
            _mexrec = _mexrec/t;
            _suret = _suret/t;
        }
        public static BigInteger Ebob(BigInteger a, BigInteger b)
        {
            
            if (a==1||b==1)
            {
                return 1;
            }
            var aa = BigInteger.Abs(a);
            var bb = BigInteger.Abs(b);
            BigInteger qaliq;
            while (aa!=bb)
            {
                if (aa>bb)
                {
                    BigInteger.DivRem(aa, bb, out qaliq);
                    aa = qaliq;
                    if (aa == 0)
                    {
                        return bb;
                    }
                }
                else
                {
                    BigInteger.DivRem(bb, aa, out qaliq);
                    bb = qaliq;
                    if (bb == 0)
                    {
                        return aa;
                    }
                }
            }
            return aa;
        }

        public override string ToString()
        {
            // 19 / 7
            var carisuret = BigInteger.Abs(Suret);
            var carimexrec = BigInteger.Abs(Mexrec);
            var res = carisuret/carimexrec;
            var resultStr = (res).ToString(DefaultCultureInfo);
            carisuret = carisuret - res*carimexrec;
            resultStr += OnluqSimvol.ToString(DefaultCultureInfo);
            carisuret = carisuret * 10;
            for (var i = 0; i < Deqiqlik; i++)
            {
                res = carisuret/carimexrec;
                carisuret -= carimexrec*res;
                carisuret = carisuret*10;
                resultStr += res.ToString();
            }
            resultStr = ((Suret < 0 && Mexrec > 0 || Suret > 0 && Mexrec < 0) ? "-" : "") + resultStr;
            return resultStr;
        }

        public void Normalize()
        {
            var yeni = ToString().ToSuperNumber();
            _mexrec = yeni.Mexrec;
            _suret = yeni.Suret;
        }
        #endregion

        #region + overloads

        public static SuperNumber operator +(SuperNumber number1, SuperNumber number2)
        {
            /*
             * a/b+c/d=(ad+bc)/bd
             */
            var newmexrec = number1.Mexrec*number2.Mexrec;
            var newsuret = number1.Suret*number2.Mexrec + number1.Mexrec*number2.Suret;
            var result = new SuperNumber(newsuret, newmexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator +(SuperNumber number1, int number2)
        {
            /*
             * a/b+c=(a+bc)/b
             */
            var newsuret = number1.Suret + number1.Mexrec*number2;
            var result = new SuperNumber(newsuret, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator +(SuperNumber number1, long number2)
        {
            /*
             * a/b+c=(a+bc)/b
             */
            var newsuret = number1.Suret + number1.Mexrec*number2;
            var result = new SuperNumber(newsuret, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator +(SuperNumber number1, ulong number2)
        {
            /*
             * a/b+c=(a+bc)/b
             */
            var newsuret = number1.Suret + number1.Mexrec*number2;
            var result = new SuperNumber(newsuret, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator +(int number1, SuperNumber number2)
        {
            var res = number2 + number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(long number1, SuperNumber number2)
        {
            var res = number2 + number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(ulong number1, SuperNumber number2)
        {
            var res = number2 + number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(SuperNumber number)
        {
            var res = new SuperNumber(-number.Suret, number.Mexrec);
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(SuperNumber number1, BigInteger number2)
        {
            var res = number1 + new SuperNumber(number2);
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(BigInteger number2, SuperNumber number1)
        {
            var res = number1 + new SuperNumber(number2);
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(SuperNumber number1, double number2)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 + num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(SuperNumber number1, float number2)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 + num2;
            return res;
        }

        public static SuperNumber operator +(SuperNumber number1, decimal number2)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 + num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(double number2, SuperNumber number1)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 + num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(float number2, SuperNumber number1)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 + num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator +(decimal number2, SuperNumber number1)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 + num2;
            res.Normalize();
            return res;
        }

        #endregion

        #region - overloads

        public static SuperNumber operator -(SuperNumber number1, SuperNumber number2)
        {
            /*
             * a/b+c/d=(ad-bc)/bd
             */
            var newmexrec = number1.Mexrec*number2.Mexrec;
            var newsuret = number1.Suret*number2.Mexrec - number1.Mexrec*number2.Suret;
            var result = new SuperNumber(newsuret, newmexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator -(BigInteger number2, SuperNumber number1)
        {
            var res = -number1 + new SuperNumber(number2);
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(SuperNumber number)
        {
            var res = new SuperNumber(-number.Suret, number.Mexrec);
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(SuperNumber number1, int number2)
        {
            /*
             * a/b-c=(a-bc)/b
             */
            var newsuret = number1.Suret - number1.Mexrec*number2;
            var result = new SuperNumber(newsuret, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator -(SuperNumber number1, long number2)
        {
            /*
             * a/b-c=(a-bc)/b
             */
            var newsuret = number1.Suret - number1.Mexrec*number2;
            var result = new SuperNumber(newsuret, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator -(SuperNumber number1, ulong number2)
        {
            /*
             * a/b-c=(a-bc)/b
             */
            var newsuret = number1.Suret - number1.Mexrec*number2;
            var result = new SuperNumber(newsuret, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }

        public static SuperNumber operator -(SuperNumber number1, BigInteger number2)
        {
            var res = number1 - new SuperNumber(number2);
            return res;
        }

        public static SuperNumber operator -(int number1, SuperNumber number2)
        {
            var res = (-number2) + number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(long number1, SuperNumber number2)
        {
            var res = (-number2) + number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(ulong number1, SuperNumber number2)
        {
            var res = (-number2) + number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(SuperNumber number1, double number2)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 - num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(SuperNumber number1, float number2)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 - num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(SuperNumber number1, decimal number2)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = number1 - num2;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(double number2, SuperNumber number1)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = num2 - number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(float number2, SuperNumber number1)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = num2 - number1;
            res.Normalize();
            return res;
        }

        public static SuperNumber operator -(decimal number2, SuperNumber number1)
        {
            var num2 = new SuperNumber(number2.ToString(DefaultCultureInfo));
            var res = num2 - number1;
            res.Normalize();
            return res;
        }

        #endregion
        
        #region * overloads

        public static SuperNumber operator *(SuperNumber number1, SuperNumber number2)
        {
            /*
             * a/b*c/d=ac/bd
             */
            var newmexrec = number1.Mexrec * number2.Mexrec;
            var newsuret = number1.Suret * number2.Suret;
            var result = new SuperNumber(newsuret, newmexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator *(SuperNumber number1, int number2)
        {
            var result = new SuperNumber(number1.Suret * number2, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator *(SuperNumber number1, long number2)
        {
            var result = new SuperNumber(number1.Suret * number2, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator *(SuperNumber number1, ulong number2)
        {
            var result = new SuperNumber(number1.Suret * number2, number1.Mexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator *(int number1, SuperNumber number2)
        {
            var res = number2 * number1;
            res.Normalize();
            return res;
        }
        public static SuperNumber operator *(long number1, SuperNumber number2)
        {
            var res = number2 * number1;
            return res;
        }
        public static SuperNumber operator *(ulong number1, SuperNumber number2)
        {
            var res = number2 * number1;
            return res;
        }
        #endregion

        #region / overloads
        public static SuperNumber operator /(SuperNumber number1, SuperNumber number2)
        {
            /*
             * a/b /  c/d=ad/bc
             */
            var newmexrec = number1.Mexrec * number2.Suret;
            var newsuret = number1.Suret * number2.Mexrec;
            var result = new SuperNumber(newsuret, newmexrec);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator /(SuperNumber number1, int number2)
        {
            var result = new SuperNumber(number1.Suret, number1.Mexrec * number2);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator /(SuperNumber number1, long number2)
        {
            var result = new SuperNumber(number1.Suret, number1.Mexrec * number2);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator /(SuperNumber number1, ulong number2)
        {
            var result = new SuperNumber(number1.Suret, number1.Mexrec * number2);
            result.IxtisarEt();
            result.Normalize();
            return result;
        }
        public static SuperNumber operator /(int number1, SuperNumber number2)
        {
            var result = new SuperNumber(number2.Mexrec * number1, number2.Suret);
            result.Normalize();
            return result;
        }
        public static SuperNumber operator /(long number1, SuperNumber number2)
        {
            var result = new SuperNumber(number2.Mexrec * number1, number2.Suret);
            result.Normalize();
            return result;
        }
        public static SuperNumber operator /(ulong number1, SuperNumber number2)
        {
            var result = new SuperNumber(number2.Mexrec * number1, number2.Suret);
            result.Normalize();
            return result;
        }
        #endregion

        public static SuperNumber operator ++(SuperNumber number)
        {
            var res = number + 1;
            res.Normalize();
            return res;
        }
        public static SuperNumber operator --(SuperNumber number)
        {
            var res = number - 1;
            res.Normalize();
            return res;
        }
       
        public static implicit operator SuperNumber(int a)
        {
            var res = new SuperNumber(a.ToString(DefaultCultureInfo));
            res.Normalize();
            return res;
        }

        #region Sqrt methods

        public static SuperNumber Sqrt(SuperNumber number)
        {
            var sqrtNumber = number/2;
            for (var i = 0; i < IterationCount; i++)
            {
                sqrtNumber = ((sqrtNumber + number/sqrtNumber)/new SuperNumber("2.0"));
            }
            return sqrtNumber;
        }

        public static SuperNumber Sqrt(int numberint)
        {
            var number = new SuperNumber(numberint.ToString(DefaultCultureInfo));
            var sqrtNumber = number/2;
            for (var i = 0; i < IterationCount; i++)
            {
                sqrtNumber = ((sqrtNumber + number/sqrtNumber)/new SuperNumber("2.0"));
            }
            return sqrtNumber;
        }

        public static SuperNumber Sqrt(long numberint)
        {
            var number = new SuperNumber(numberint.ToString(DefaultCultureInfo));
            var sqrtNumber = number/2;
            for (var i = 0; i < IterationCount; i++)
            {
                sqrtNumber = ((sqrtNumber + number/sqrtNumber)/new SuperNumber("2.0"));
            }
            return sqrtNumber;
        }

        public static SuperNumber Sqrt(ulong numberint)
        {
            var number = new SuperNumber(numberint.ToString(DefaultCultureInfo));
            var sqrtNumber = number/2;
            for (var i = 0; i < IterationCount; i++)
            {
                sqrtNumber = ((sqrtNumber + number/sqrtNumber)/new SuperNumber("2.0"));
            }
            return sqrtNumber;
        }

        public static SuperNumber Sqrt(double numberint)
        {
            var number = new SuperNumber(numberint.ToString(DefaultCultureInfo));
            var sqrtNumber = number/2;
            for (var i = 0; i < IterationCount; i++)
            {
                sqrtNumber = ((sqrtNumber + number/sqrtNumber)/new SuperNumber("2.0"));
                sqrtNumber.Normalize();
            }
            return sqrtNumber;

        }

        public static SuperNumber Sqrt(decimal x)
        {
            return Sqrt(x.ToString(DefaultCultureInfo).ToSuperNumber());
        }

        public static SuperNumber Sqrt(BigInteger number)
        {
            return Sqrt(new SuperNumber(number));
        }

        #endregion

        #region exp methods
        private static SuperNumber exp_0_1(SuperNumber x)
        {
            /*
             * s=1
             * s=s*(1+x/4)  s=1+x/4
             * s=s*()
             */
            //exp(x)=1+x*(1+x/2*(1+x/3*(1+x/4)))

            var i = IterationCount;
            SuperNumber s = 1;
            while (i>0)
            {
                s = 1 + s*x/i;
                i--;
            }
            return s;
        }
        
        private static SuperNumber exp_int(BigInteger quvvet)
        {
            var bigInteger = quvvet;
            SuperNumber y = 1;
            var z = E;
            BigInteger qaliq;
        A2:
            bigInteger = BigInteger.DivRem(bigInteger, 2, out qaliq);
            if (qaliq == 0)
            {
                goto A5;
            }
            y = y * z;
            if (bigInteger == 0)
            {
                return y;
            }
        A5:
            z = z * z;
            goto A2;
        }

        public static SuperNumber Exp(SuperNumber xNumber)
        {
            return null;
        }
        #endregion
       
    }
}
