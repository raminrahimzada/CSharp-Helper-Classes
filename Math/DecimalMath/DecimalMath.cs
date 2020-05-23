using System;

// ReSharper disable once IdentifierTypo
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
        public const decimal Pi = 3.14159265358979323846264338327950288419716939937510M;

        /// <summary>
        /// represents PI
        /// </summary>
        public const decimal Epsilon = 0.0000000000000000001M;

        /// <summary>
        /// represents 2*PI
        /// </summary>
        private const decimal PIx2 = 6.28318530717958647692528676655900576839433879875021M;

        /// <summary>
        /// represents E
        /// </summary>
        public const decimal E = 2.7182818284590452353602874713526624977572470936999595749M;

        /// <summary>
        /// represents PI/2
        /// </summary>
        private const decimal PIdiv2 = 1.570796326794896619231321691639751442098584699687552910487M;

        /// <summary>
        /// represents PI/4
        /// </summary>
        private const decimal PIdiv4 = 0.785398163397448309615660845819875721049292349843776455243M;

        /// <summary>
        /// represents 1.0/E
        /// </summary>
        private const decimal Einv = 0.3678794411714423215955237701614608674458111310317678M;

        /// <summary>
        /// log(10,E) factor
        /// </summary>
        private const decimal Log10Inv = 0.434294481903251827651128918916605082294397005803666566114M;

        /// <summary>
        /// Zero
        /// </summary>
        public const decimal Zero = 0.0M;

        /// <summary>
        /// One
        /// </summary>
        public const decimal One = 1.0M;

        /// <summary>
        /// Represents 0.5M
        /// </summary>
        private const decimal Half = 0.5M;

        /// <summary>
        /// Max iterations count in Taylor series
        /// </summary>
        private const int MaxIteration = 100;

        /// <summary>
        /// Analogy of Math.Exp method
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Exp(decimal x)
        {
            var count = 0;

            if (x > One)
            {
                count = decimal.ToInt32(decimal.Truncate(x));
                x -= decimal.Truncate(x);
            }

            if (x < Zero)
            {
                count = decimal.ToInt32(decimal.Truncate(x) - 1);
                x = One + (x - decimal.Truncate(x));
            }

            var iteration = 1;
            var result = One;
            var factorial = One;
            decimal cachedResult;
            do
            {
                cachedResult = result;
                factorial *= x / iteration++;
                result += factorial;
            } while (cachedResult != result);

            if (count == 0)
                return result;
            return result * PowerN(E, count);
        }

        /// <summary>
        /// Analogy of Math.Pow method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pow"></param>
        /// <returns></returns>
        public static decimal Power(decimal value, decimal pow)
        {
            if (pow == Zero) return One;
            if (pow == One) return value;
            if (value == One) return One;

            if (value == Zero && pow == Zero) return One;

            if (value == Zero)
            {
                if (pow > Zero)
                {
                    return Zero;
                }

                throw new Exception("Invalid Operation: zero base and negative power");
            }

            if (pow == -One) return One / value;

            var isPowerInteger = IsInteger(pow);
            if (value < Zero && !isPowerInteger)
            {
                throw new Exception("Invalid Operation: negative base and non-integer power");
            }

            if (isPowerInteger && value > Zero)
            {
                int powerInt = (int)pow;
                return PowerN(value, powerInt);
            }

            if (isPowerInteger && value < Zero)
            {
                int powerInt = (int)pow;
                if (powerInt % 2 == 0)
                {
                    return Exp(pow * Log(-value));
                }

                return -Exp(pow * Log(-value));
            }

            return Exp(pow * Log(value));
        }

        private static bool IsInteger(decimal value)
        {
            var longValue = (long)value;
            return Abs(value - longValue) <= Epsilon;
        }

        /// <summary>
        /// Power to the integer value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static decimal PowerN(decimal value, int power)
        {
            while (true)
            {
                if (power == Zero) return One;
                if (power < Zero)
                {
                    value = One / value;
                    power = -power;
                    continue;
                }

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

                    current *= current; // value^i -> value^(2*i)
                    q >>= 1;
                }

                return prod;
            }
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
            if (x <= Zero)
            {
                throw new ArgumentException("x must be greater than zero");
            }
            var count = 0;
            while (x >= One)
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
            if (x == Zero) return count;
            var result = Zero;
            var iteration = 0;
            var y = One;
            var cacheResult = result - One;
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
            //truncating to  [-2*PI;2*PI]
            TruncateToPeriodicInterval(ref x);
            
            // now x in (-2pi,2pi)
            if (x >= Pi && x <= PIx2)
            {
                return -Cos(x - Pi);
            }
            if (x >= -PIx2 && x <= -Pi)
            {
                return -Cos(x + Pi);
            }

            x *= x;
            //y=1-x/2!+x^2/4!-x^3/6!...
            var xx = -x * Half;
            var y = One + xx;
            var cachedY = y - One;//init cache  with different value
            for (var i = 1; cachedY != y && i < MaxIteration; i++)
            {
                cachedY = y;
                decimal factor = i * ((i << 1) + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
                factor = -Half / factor;
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
            var cos = Cos(x);
            if (cos == Zero) throw new ArgumentException(nameof(x));
            //calculate sin using cos
            var sin = CalculateSinFromCos(x, cos);
            return sin / cos;
        }
        /// <summary>
        /// Helper function for calculating sin(x) from cos(x)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="cos"></param>
        /// <returns></returns>
        private static decimal CalculateSinFromCos(decimal x,decimal cos)
        {
            var moduleOfSin = Sqrt(One - (cos * cos));
            var sineIsPositive = IsSignOfSinePositive(x);
            if (sineIsPositive) return moduleOfSin;
            return -moduleOfSin;
        }
        /// <summary>
        /// Analogy of Math.Sin
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            return CalculateSinFromCos(x, cos);
        }


        /// <summary>
        /// Truncates to  [-2*PI;2*PI]
        /// </summary>
        /// <param name="x"></param>
        private static void TruncateToPeriodicInterval(ref decimal x)
        {
            while (x >= PIx2)
            {
                var divide = Math.Abs(decimal.ToInt32(x / PIx2));
                x -= divide * PIx2;
            }

            while (x <= -PIx2)
            {
                var divide = Math.Abs(decimal.ToInt32(x / PIx2));
                x += divide * PIx2;
            }
        }


        private static bool IsSignOfSinePositive(decimal x)
        {
            //truncating to  [-2*PI;2*PI]
            TruncateToPeriodicInterval(ref x);
            
            //now x in [-2*PI;2*PI]
            if (x >= -PIx2 && x <= -Pi) return true;
            if (x >= -Pi && x <= Zero) return false;
            if (x >= Zero && x <= Pi) return true;
            if (x >= Pi && x <= PIx2) return false;

            //will not be reached
            throw new ArgumentException(nameof(x));
        }

        /// <summary>
        /// Analogy of Math.Sqrt
        /// </summary>
        /// <param name="x"></param>
        /// <param name="epsilon">lasts iteration while error less than this epsilon</param>
        /// <returns></returns>
        public static decimal Sqrt(decimal x, decimal epsilon = Zero)
        {
            if (x < Zero) throw new OverflowException("Cannot calculate square root from a negative number");
            //initial approximation
            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == Zero) return Zero;
                current = (previous + x / previous) * Half;
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
            var yy = One / y;
            return (y - yy) * Half;
        }

        /// <summary>
        /// Analogy of Math.Cosh
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y + yy) * Half;
        }

        /// <summary>
        /// Analogy of Math.Sign
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int Sign(decimal x)
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
            var yy = One / y;
            return (y - yy) / (y + yy);
        }

        /// <summary>
        /// Analogy of Math.Abs
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static decimal Abs(decimal x)
        {
            if (x <= Zero)
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
            if (x == Zero) return Zero;
            if (x == One) return PIdiv2;
            //asin function is odd function
            if (x < Zero) return -Asin(-x);

            //my optimize trick here

            // used a math formula to speed up :
            // asin(x)=0.5*(pi/2-asin(1-2*x*x)) 
            // if x>=0 is true

            var newX = One - 2 * x * x;

            //for calculating new value near to zero than current
            //because we gain more speed with values near to zero
            if (Abs(x) > Abs(newX))
            {
                var t = Asin(newX);
                return Half * (PIdiv2 - t);
            }
            var y = Zero;
            var result = x;
            decimal cachedResult;
            var i = 1;
            y += result;
            var xx = x * x;
            do
            {
                cachedResult = result;
                result *= xx * (One - Half / (i));
                y += result / ((i << 1) + 1);
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
            return Asin(x / Sqrt(One + x * x));
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
            if (x < Zero) return Pi - Acos(-x);
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
        public static decimal Atan2(decimal y, decimal x)
        {
            if (x > Zero)
            {
                return ATan(y / x);
            }
            if (x < Zero && y >= Zero)
            {
                return ATan(y / x) + Pi;
            }
            if (x < Zero && y < Zero)
            {
                return ATan(y / x) - Pi;
            }
            if (x == Zero && y > Zero)
            {
                return PIdiv2;
            }
            if (x == Zero && y < Zero)
            {
                return -PIdiv2;
            }
            throw new ArgumentException("invalid atan2 arguments");
        }
    }
}
