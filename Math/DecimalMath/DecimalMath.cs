using System;

namespace raminrahimzada
{
    /// <summary>
    /// Analogy of Syatem.Math class for decimal types 
    /// </summary>
    public static class DecimalMath
    {
        /// <summary>
        /// represents PI
        /// </summary>
        public static readonly decimal PI = 3.14159265358979323846264338327950288419716939937510M;
        /// <summary>
        /// represents 2*PI
        /// </summary>
        public static readonly decimal PIx2 = 6.28318530717958647692528676655900576839433879875021M;
        /// <summary>
        /// represents E
        /// </summary>
        public static readonly decimal E = 2.7182818284590452353602874713526624977572470936999595749M;
        /// <summary>
        /// represents PI/2
        /// </summary>
        public static readonly decimal PIdiv2 = 1.570796326794896619231321691639751442098584699687552910487M;
        /// <summary>
        /// represents PI/4
        /// </summary>
        public static readonly decimal PIdiv4 = 0.785398163397448309615660845819875721049292349843776455243M;
        /// <summary>
        /// represents 1.0/E
        /// </summary>
        public static readonly decimal Einv = 0.3678794411714423215955237701614608674458111310317678M;
        /// <summary>
        /// represents Logarithm 2 from base E
        /// </summary>
        public static readonly decimal LOG2 = 0.693147180559945309417232121458176568075500134360255254120M;
        /// <summary>
        /// log(10,E) factor
        /// </summary>
        public static readonly decimal Log10Inv = 0.434294481903251827651128918916605082294397005803666566114M;
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly decimal Zero = 0.0M;
        /// <summary>
        /// One
        /// </summary>
        public static readonly decimal One = 1.0M;

        /// <summary>
        /// Max iterations count in Taylor series
        /// </summary>
        public static int MaxIteration = 100;
        /// <summary>
        /// Analogy of Math.Exp method
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Exp(decimal x)
        {
            int count = 0;
            while (x > One)
            {
                x--;
                count++;
            }
            while (x < Zero)
            {
                x++;
                count--;
            }
            int iteration = 1;
            decimal result = One;
            decimal fatorial = One;
            decimal cachedResult;
            do
            {
                cachedResult = result;
                fatorial *= x / iteration++;
                result += fatorial;
            } while (cachedResult != result);
            if (count != 0) result = result * PowerN(E, count);
            return result;
        }
        /// <summary>
        /// Analogy of Math.Pow method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pow"></param>
        /// <returns></returns>
        public static decimal Power(decimal value, decimal pow)
        {
            return Exp(pow * Log(value));
        }
        /// <summary>
        /// Power to the integer value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static decimal PowerN(decimal value, int power)
        {
            if (power == 0) return One;
            if (power < 0) return PowerN(1.0M / value, -power);
            
            var q = power;
            var prod = One;
            var current = value;
            while (q > 0)
            {
                if (q % 2 == 1)
                {
                    // detects the 1s in the binary expression of power
                    prod = current * prod; // picks up the relevant power
                    q--;
                }
                current = current * current; // value^i -> value^(2*i)
                q = q / 2;
            }

            return prod;
        }
         /// <summary>
        /// Analogy of Math.Log10
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Log10(decimal x)
        {
            return Log(x) * Log10Inv;
        }
        /// <summary>
        /// Analogy of Math.Log
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Log(decimal x)
        {
            if (x <= 0)
            {
                throw new ArgumentException("x must be greater than zero");
            }
            int count = 0;
            while (x >= 1)
            {
                x *= Einv;
                count++;
            }
            while (x <= Einv)
            {
                x *= E;
                count--;
            }
            x--;
            if (x == 0) return count;
            decimal result = 0.0M;
            int iteration = 0;
            decimal y = 1.0M;
            decimal cacheResult = result - 1.0M;
            while (cacheResult != result && iteration < MaxIteration)
            {
                iteration++;
                cacheResult = result;
                y *= -x;
                result += y / iteration;
            }
            return count - result;
        }
        /// <summary>
        /// Analogy of Math.Cos
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Cos(decimal x)
        {
            while (x > PIx2)
            {
                x -= PIx2;
            }
            while (x < -PIx2)
            {
                x += PIx2;
            }
            // now x in (-2pi,2pi)
            if (x >= PI && x <= PIx2)
            {
                return -Cos(x - PI);
            }
            if (x >= -PIx2 && x <= -PI)
            {
                return -Cos(x + PI);
            }
            x = x * x;
            //y=1-x/2!+x^2/4!-x^3/6!...
            decimal xx = -x * 0.5M;
            decimal y = 1.0M + xx;
            decimal cachedY = y - 1.0M;//init cache  with different value
            for (int i = 1; cachedY != y && i < MaxIteration; i++)
            {
                cachedY = y;
                decimal factor = i * (i + i + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
                factor = -0.5M / factor;
                xx *= x * factor;
                y += xx;
            }
            return y;
        }
        /// <summary>
        /// Analogy of Math.Tan
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Tan(decimal x)
        {
            return Sin(x) / Cos(x);
        }
        /// <summary>
        /// Analogy of Math.Sin
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            var real = Math.Sin((double)x);
            return Sqrt(1.0M - cos * cos) * Math.Sign(real);
        }
        /// <summary>
        /// Analogy of Math.Sqrt
        /// </summary>
        /// <param name="x"></param>
        /// <param name="epsilon">lasts iteration while error less than this epsilon</param>
        /// <returns></returns>
        public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");
            //initial approximation
            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0.0M;
                current = (previous + x / previous) * 0.5M;
            } while (Abs(previous - current) > epsilon);
            return current;
        }
        /// <summary>
        /// Analogy of Math.Sinh
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Sinh(decimal x)
        {
            var y = Exp(x);
            var yy = 1.0M / y;
            return (y - yy) / 2.0M;
        }
        /// <summary>
        /// Analogy of Math.Cosh
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = 1.0M / y;
            return (y + yy) / 2.0M;
        }
        /// <summary>
        /// Analogy of Math.Sign
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Sign(decimal x)
        {
            return x < Zero ? -1 : (x > Zero ? 1 : 0);
        }
        /// <summary>
        /// Analogy of Math.Tanh
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Tanh(decimal x)
        {
            var y = Exp(x);
            var yy = 1.0M / y;
            return (y - yy) / (y + yy);
        }

        /// <summary>
        /// Analogy of Math.Abs
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Abs(decimal x)
        {
            if (x<=Zero)
            {
                return -x;
            }
            return x;
        }

        /// <summary>
        /// Analogy of Math.Asin
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Asin(decimal x)
        {
            if (x > One || x < -One)
            {
                throw new ArgumentException("x must be in [-1,1]");
            }
            //known values
            if (x == Zero) return 0;
            if (x == One) return PIdiv2;
            //asin function is odd function
            if (x < Zero) return -Asin(-x);

            //my optimize trick here

            // used a math formula to speed up :
            // asin(x)=0.5*(pi/2-asin(1-2*x*x)) 
            // if x>=0 is true

            var newX = 1 - 2 * x * x;

            //for calculating new value near to zero than current
            //because we gain more speed with values near to zero
            if (Abs(x) >Abs(newX))
            {
                var t = Asin(newX);
                return 0.5M * (PIdiv2 - t);
            }
            decimal y = 0;
            decimal result = x;
            decimal cachedResult;
            int i = 1;
            y += result;
            var xx = x * x;
            do
            {
                cachedResult = result;
                result *= xx * (1 - 1.0M / (2.0M * i));
                y += result / (2 * i + 1);
                i++;
            } while (cachedResult != result);
            return y;
        }
        /// <summary>
        /// Analogy of Math.Atan
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal ATan(decimal x)
        {
            if (x == Zero) return Zero;
            if (x == One) return PIdiv4;
            return Asin(x / Sqrt(1 + x * x));
        }
        /// <summary>
        /// Analogy of Math.Acos
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Acos(decimal x)
        {
            if (x == Zero) return PIdiv2;
            if (x == One) return Zero;
            if (x < Zero) return PI - Acos(-x);
            return PIdiv2 - Asin(x);
        }

        /// <summary>
        /// Analogy of Math.Atan2
        /// for more see this
        /// <seealso cref="http://i.imgur.com/TRLjs8R.png"/>
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Atan2(decimal y,decimal x)
        {
            if (x>Zero)
            {
                return ATan(y / x);
            }
            if (x < Zero && y >= Zero)
            {
                return ATan(y / x) + PI;
            }
            if (x < Zero && y < Zero)
            {
                return ATan(y / x) - PI;
            }
            if (x == Zero && y > Zero)
            {
                return  PIdiv2;
            }
            if (x == Zero && y < Zero)
            {
                return -PIdiv2;
            }
            throw new ArgumentException("invalid atan2 arguments");
        }
    }
}
