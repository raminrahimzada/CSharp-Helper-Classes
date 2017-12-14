// http://www.fractal-landscapes.co.uk/bigint.html

using System;

namespace BigNum
{
    /// <summary>
    /// An arbitrary-precision floating-point class
    /// 
    /// Format:
    /// Each number is stored as an exponent (32-bit signed integer), and a mantissa
    /// (n-bit) BigInteger. The sign of the number is stored in the BigInteger
    /// 
    /// Applicability and Performance:
    /// This class is designed to be used for small extended precisions. It may not be
    /// safe (and certainly won't be fast) to use it with mixed-precision arguments.
    /// It does support, but will not be efficient for, numbers over around 2048 bits.
    /// 
    /// Notes:
    /// All conversions to and from strings are slow.
    /// 
    /// Conversions from simple integer types Int32, Int64, UInt32, UInt64 are performed
    /// using the appropriate constructor, and are relatively fast.
    /// 
    /// The class is written entirely in managed C# code, with not native or managed
    /// assembler. The use of native assembler would speed up the multiplication operations
    /// many times over, and therefore all higher-order operations too.
    /// </summary>
    public class BigFloat
    {
        /// <summary>
        /// Floats can have 4 special value types:
        /// 
        /// NaN: Not a number (cannot be changed using any operations)
        /// Infinity: Positive infinity. Some operations e.g. Arctan() allow this input.
        /// -Infinity: Negative infinity. Some operations allow this input.
        /// Zero
        /// </summary>
        public enum SpecialValueType
        {
            /// <summary>
            /// Not a special value
            /// </summary>
            NONE = 0,
            /// <summary>
            /// Zero
            /// </summary>
            ZERO,
            /// <summary>
            /// Positive infinity
            /// </summary>
            INF_PLUS,
            /// <summary>
            /// Negative infinity
            /// </summary>
            INF_MINUS,
            /// <summary>
            /// Not a number
            /// </summary>
            NAN
        }

        /// <summary>
        /// This affects the ToString() method. 
        /// 
        /// With Trim rounding, all insignificant zero digits are drip
        /// </summary>
        public enum RoundingModeType
        {
            /// <summary>
            /// Trim non-significant zeros from ToString output after rounding
            /// </summary>
            TRIM,
            /// <summary>
            /// Keep all non-significant zeroes in ToString output after rounding
            /// </summary>
            EXACT
        }

        /// <summary>
        /// A wrapper for the signed exponent, avoiding overflow.
        /// </summary>
        protected struct ExponentAdaptor
        {
            /// <summary>
            /// The 32-bit exponent
            /// </summary>
            public Int32 exponent
            {
                get { return expValue; }
                set { expValue = value; }
            }

            /// <summary>
            /// Implicit cast to Int32
            /// </summary>
            public static implicit operator Int32(ExponentAdaptor adaptor)
            {
                return adaptor.expValue;
            }

            /// <summary>
            /// Implicit cast from Int32 to ExponentAdaptor
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static implicit operator ExponentAdaptor(Int32 value)
            {
                ExponentAdaptor adaptor = new ExponentAdaptor();
                adaptor.expValue = value;
                return adaptor;
            }

            /// <summary>
            /// Overloaded increment operator
            /// </summary>
            public static ExponentAdaptor operator ++(ExponentAdaptor adaptor)
            {
                adaptor = adaptor + 1;
                return adaptor;
            }

            /// <summary>
            /// Overloaded decrement operator
            /// </summary>
            public static ExponentAdaptor operator --(ExponentAdaptor adaptor)
            {
                adaptor = adaptor - 1;
                return adaptor;
            }

            /// <summary>
            /// Overloaded addition operator
            /// </summary>
            public static ExponentAdaptor operator +(ExponentAdaptor a1, ExponentAdaptor a2)
            {
                if (a1.expValue == Int32.MaxValue) return a1;

                Int64 temp = (Int64)a1.expValue;
                temp += (Int64)(a2.expValue);

                if (temp > (Int64)Int32.MaxValue)
                {
                    a1.expValue = Int32.MaxValue;
                }
                else if (temp < (Int64)Int32.MinValue)
                {
                    a1.expValue = Int32.MinValue;
                }
                else
                {
                    a1.expValue = (Int32)temp;
                }

                return a1;
            }

            /// <summary>
            /// Overloaded subtraction operator
            /// </summary>
            public static ExponentAdaptor operator -(ExponentAdaptor a1, ExponentAdaptor a2)
            {
                if (a1.expValue == Int32.MaxValue) return a1;

                Int64 temp = (Int64)a1.expValue;
                temp -= (Int64)(a2.expValue);

                if (temp > (Int64)Int32.MaxValue)
                {
                    a1.expValue = Int32.MaxValue;
                }
                else if (temp < (Int64)Int32.MinValue)
                {
                    a1.expValue = Int32.MinValue;
                }
                else
                {
                    a1.expValue = (Int32)temp;
                }

                return a1;
            }

            /// <summary>
            /// Overloaded multiplication operator
            /// </summary>
            public static ExponentAdaptor operator *(ExponentAdaptor a1, ExponentAdaptor a2)
            {
                if (a1.expValue == Int32.MaxValue) return a1;

                Int64 temp = (Int64)a1.expValue;
                temp *= (Int64)a2.expValue;

                if (temp > (Int64)Int32.MaxValue)
                {
                    a1.expValue = Int32.MaxValue;
                }
                else if (temp < (Int64)Int32.MinValue)
                {
                    a1.expValue = Int32.MinValue;
                }
                else
                {
                    a1.expValue = (Int32)temp;
                }

                return a1;
            }

            /// <summary>
            /// Overloaded division operator
            /// </summary>
            public static ExponentAdaptor operator /(ExponentAdaptor a1, ExponentAdaptor a2)
            {
                if (a1.expValue == Int32.MaxValue) return a1;

                ExponentAdaptor res = new ExponentAdaptor();
                res.expValue = a1.expValue / a2.expValue;
                return res;
            }

            /// <summary>
            /// Overloaded right-shift operator
            /// </summary>
            public static ExponentAdaptor operator >>(ExponentAdaptor a1, int shift)
            {
                if (a1.expValue == Int32.MaxValue) return a1;

                ExponentAdaptor res = new ExponentAdaptor();
                res.expValue = a1.expValue >> shift;
                return res;
            }

            /// <summary>
            /// Overloaded left-shift operator
            /// </summary>
            /// <param name="a1"></param>
            /// <param name="shift"></param>
            /// <returns></returns>
            public static ExponentAdaptor operator <<(ExponentAdaptor a1, int shift)
            {
                if (a1.expValue == 0) return a1;

                ExponentAdaptor res = new ExponentAdaptor();
                res.expValue = a1.expValue;

                if (shift > 31)
                {
                    res.expValue = Int32.MaxValue;
                }
                else
                {
                    Int64 temp = a1.expValue;
                    temp = temp << shift;

                    if (temp > (Int64)Int32.MaxValue)
                    {
                        res.expValue = Int32.MaxValue;
                    }
                    else if (temp < (Int64)Int32.MinValue)
                    {
                        res.expValue = Int32.MinValue;
                    }
                    else
                    {
                        res.expValue = (Int32)temp;
                    }
                }

                return res;
            }

            private Int32 expValue;
        }

        //************************ Constructors **************************

        /// <summary>
        /// Constructs a 128-bit BigFloat
        /// 
        /// Sets the value to zero
        /// </summary>
        static BigFloat()
        {
            RoundingDigits = 3;
            RoundingMode = RoundingModeType.TRIM;
            scratch = new BigInt(new PrecisionSpec(128, PrecisionSpec.BaseType.BIN));
        }

        /// <summary>
        /// Constructs a BigFloat of the required precision
        /// 
        /// Sets the value to zero
        /// </summary>
        /// <param name="mantissaPrec"></param>
        public BigFloat(PrecisionSpec mantissaPrec)
        {
            Init(mantissaPrec);
        }

        /// <summary>
        /// Constructs a big float from a UInt32 to the required precision
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(UInt32 value, PrecisionSpec mantissaPrec)
        {
            int mbWords = ((mantissaPrec.NumBits) >> 5);
            if ((mantissaPrec.NumBits & 31) != 0) mbWords++;
            int newManBits = mbWords << 5;

            //For efficiency, we just use a 32-bit exponent
            exponent = 0;

            mantissa = new BigInt(value, new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
            //scratch = new BigInt(mantissa.Precision);

            int bit = BigInt.GetMSB(value);
            if (bit == -1) return;

            int shift = mantissa.Precision.NumBits - (bit + 1);
            mantissa.LSH(shift);
            exponent = bit;
        }

        /// <summary>
        /// Constructs a BigFloat from an Int32 to the required precision
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(Int32 value, PrecisionSpec mantissaPrec)
        {
            int mbWords = ((mantissaPrec.NumBits) >> 5);
            if ((mantissaPrec.NumBits & 31) != 0) mbWords++;
            int newManBits = mbWords << 5;

            //For efficiency, we just use a 32-bit exponent
            exponent = 0;
            UInt32 uValue;
            
            if (value < 0)
            {
                if (value == Int32.MinValue)
                {
                    uValue = 0x80000000;
                }
                else
                {
                    uValue = (UInt32)(-value);
                }
            }
            else
            {
                uValue = (UInt32)value;
            }

            mantissa = new BigInt(value, new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
            //scratch = new BigInt(new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));

            int bit = BigInt.GetMSB(uValue);
            if (bit == -1) return;

            int shift = mantissa.Precision.NumBits - (bit + 1);
            mantissa.LSH(shift);
            exponent = bit;
        }

        /// <summary>
        /// Constructs a BigFloat from a 64-bit integer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(Int64 value, PrecisionSpec mantissaPrec)
        {
            int mbWords = ((mantissaPrec.NumBits) >> 5);
            if ((mantissaPrec.NumBits & 31) != 0) mbWords++;
            int newManBits = mbWords << 5;

            //For efficiency, we just use a 32-bit exponent
            exponent = 0;
            UInt64 uValue;

            if (value < 0)
            {
                if (value == Int64.MinValue)
                {
                    uValue = 0x80000000;
                }
                else
                {
                    uValue = (UInt64)(-value);
                }
            }
            else
            {
                uValue = (UInt64)value;
            }

            mantissa = new BigInt(value, new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
            //scratch = new BigInt(new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));

            int bit = BigInt.GetMSB(uValue);
            if (bit == -1) return;

            int shift = mantissa.Precision.NumBits - (bit + 1);
            if (shift > 0)
            {
                mantissa.LSH(shift);
            }
            else
            {
                mantissa.SetHighDigit((uint)(uValue >> (-shift)));
            }
            exponent = bit;
        }

        /// <summary>
        /// Constructs a BigFloat from a 64-bit unsigned integer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(UInt64 value, PrecisionSpec mantissaPrec)
        {
            int mbWords = ((mantissaPrec.NumBits) >> 5);
            if ((mantissaPrec.NumBits & 31) != 0) mbWords++;
            int newManBits = mbWords << 5;

            //For efficiency, we just use a 32-bit exponent
            exponent = 0;

            int bit = BigInt.GetMSB(value);

            mantissa = new BigInt(value, new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
            //scratch = new BigInt(mantissa.Precision);

            int shift = mantissa.Precision.NumBits - (bit + 1);
            if (shift > 0)
            {
                mantissa.LSH(shift);
            }
            else
            {
                mantissa.SetHighDigit((uint)(value >> (-shift)));
            }
            exponent = bit;
        }

        /// <summary>
        /// Constructs a BigFloat from a BigInt, using the specified precision
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(BigInt value, PrecisionSpec mantissaPrec)
        {
            if (value.IsZero())
            {
                Init(mantissaPrec);
                SetZero();
                return;
            }

            mantissa = new BigInt(value, mantissaPrec);
            exponent = BigInt.GetMSB(value);
            mantissa.Normalise();
        }

        /// <summary>
        /// Construct a BigFloat from a double-precision floating point number
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(double value, PrecisionSpec mantissaPrec)
        {
            if (value == 0.0)
            {
                Init(mantissaPrec);
                return;
            }

            bool sign = (value < 0) ? true : false;

            long bits = BitConverter.DoubleToInt64Bits(value);
            // Note that the shift is sign-extended, hence the test against -1 not 1
            int valueExponent = (int)((bits >> 52) & 0x7ffL);
            long valueMantissa = bits & 0xfffffffffffffL;

            //The mantissa is stored with the top bit implied.
            valueMantissa = valueMantissa | 0x10000000000000L;

            //The exponent is biased by 1023.
            exponent = valueExponent - 1023;

            //Round the number of bits to the nearest word.
            int mbWords = ((mantissaPrec.NumBits) >> 5);
            if ((mantissaPrec.NumBits & 31) != 0) mbWords++;
            int newManBits = mbWords << 5;

            mantissa = new BigInt(new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
            //scratch = new BigInt(new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));

            if (newManBits >= 64)
            {
                //The mantissa is 53 bits now, so add 11 to put it in the right place.
                mantissa.SetHighDigits(valueMantissa << 11);
            }
            else
            {
                //To get the top word of the mantissa, shift up by 11 and down by 32 = down by 21
                mantissa.SetHighDigit((uint)(valueMantissa >> 21));
            }

            mantissa.Sign = sign;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="value"></param>
        public BigFloat(BigFloat value)
        {
            Init(value.mantissa.Precision);
            exponent = value.exponent;
            mantissa.Assign(value.mantissa);
        }

        /// <summary>
        /// Copy Constructor - constructs a new BigFloat with the specified precision, copying the old one.
        /// 
        /// The value is rounded towards zero in the case where precision is decreased. The Round() function
        /// should be used beforehand if a correctly rounded result is required.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(BigFloat value, PrecisionSpec mantissaPrec)
        {
            Init(mantissaPrec);
            exponent = value.exponent;
            if (mantissa.AssignHigh(value.mantissa)) exponent++;
        }

        /// <summary>
        /// Constructs a BigFloat from a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mantissaPrec"></param>
        public BigFloat(string value, PrecisionSpec mantissaPrec)
        {
            Init(mantissaPrec);

            PrecisionSpec extendedPres = new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN);
            BigFloat ten = new BigFloat(10, extendedPres);
            BigFloat iPart = new BigFloat(extendedPres);
            BigFloat fPart = new BigFloat(extendedPres);
            BigFloat tenRCP = ten.Reciprocal();

            if (value.Contains(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NaNSymbol))
            {
                SetNaN();
                return;
            }
            else if (value.Contains(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PositiveInfinitySymbol))
            {
                SetInfPlus();
                return;
            }
            else if (value.Contains(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NegativeInfinitySymbol))
            {
                SetInfMinus();
                return;
            }

            string decimalpoint = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            char[] digitChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };

            //Read in the integer part up the the decimal point.
            bool sign = false;
            value = value.Trim();

            int i = 0;

            if (value.Length > i && value[i] == '-')
            {
                sign = true;
                i++;
            }

            if (value.Length > i && value[i] == '+')
            {
                i++;
            }

            for ( ; i < value.Length; i++)
            {
                //break on decimal point
                if (value[i] == decimalpoint[0]) break;

                int digit = Array.IndexOf(digitChars, value[i]);
                if (digit < 0) break;

                //Ignore place separators (assumed either , or .)
                if (digit > 9) continue;

                if (i > 0) iPart.Mul(ten);
                iPart.Add(new BigFloat(digit, extendedPres));
            }

            //If we've run out of characters, assign everything and return
            if (i == value.Length)
            {
                iPart.mantissa.Sign = sign;
                exponent = iPart.exponent;
                if (mantissa.AssignHigh(iPart.mantissa)) exponent++;
                return;
            }

            //Assign the characters after the decimal point to fPart
            if (value[i] == '.' && i < value.Length - 1)
            {
                BigFloat RecipToUse = new BigFloat(tenRCP);

                for (i++; i < value.Length; i++)
                {
                    int digit = Array.IndexOf(digitChars, value[i]);
                    if (digit < 0) break;
                    BigFloat temp = new BigFloat(digit, extendedPres);
                    temp.Mul(RecipToUse);
                    RecipToUse.Mul(tenRCP);
                    fPart.Add(temp);
                }
            }

            //If we're run out of characters, add fPart and iPart and return
            if (i == value.Length)
            {
                iPart.Add(fPart);
                iPart.mantissa.Sign = sign;
                exponent = iPart.exponent;
                if (mantissa.AssignHigh(iPart.mantissa)) exponent++;
                return;
            }

            if (value[i] == '+' || value[i] == '-') i++;

            if (i == value.Length)
            {
                iPart.Add(fPart);
                iPart.mantissa.Sign = sign;
                exponent = iPart.exponent;
                if (mantissa.AssignHigh(iPart.mantissa)) exponent++;
                return;
            }

            //Look for exponential notation.
            if ((value[i] == 'e' || value[i] == 'E') && i < value.Length - 1)
            {
                //Convert the exponent to an int.
                int exp;

                try
                {
					exp = System.Convert.ToInt32(new string(value.ToCharArray()));// i + 1, value.Length - (i + 1))));
                }
                catch (Exception)
                {
                    iPart.Add(fPart);
                    iPart.mantissa.Sign = sign;
                    exponent = iPart.exponent;
                    if (mantissa.AssignHigh(iPart.mantissa)) exponent++;
                    return;
                }

                //Raise or lower 10 to the power of the exponent
                BigFloat acc = new BigFloat(1, extendedPres);
                BigFloat temp = new BigFloat(1, extendedPres);

                int powerTemp = exp;

                BigFloat multiplierToUse;

                if (exp < 0)
                {
                    multiplierToUse = new BigFloat(tenRCP);
                    powerTemp = -exp;
                }
                else
                {
                    multiplierToUse = new BigFloat(ten);
                }

                //Fast power function
                while (powerTemp != 0)
                {
                    temp.Mul(multiplierToUse);
                    multiplierToUse.Assign(temp);

                    if ((powerTemp & 1) != 0)
                    {
                        acc.Mul(temp);
                    }

                    powerTemp >>= 1;
                }

                iPart.Add(fPart);
                iPart.Mul(acc);
                iPart.mantissa.Sign = sign;
                exponent = iPart.exponent;
                if (mantissa.AssignHigh(iPart.mantissa)) exponent++;

                return;
            }

            iPart.Add(fPart);
            iPart.mantissa.Sign = sign;
            exponent = iPart.exponent;
            if (mantissa.AssignHigh(iPart.mantissa)) exponent++;

        }

        private void Init(PrecisionSpec mantissaPrec)
        {
            int mbWords = ((mantissaPrec.NumBits) >> 5);
            if ((mantissaPrec.NumBits & 31) != 0) mbWords++;
            int newManBits = mbWords << 5;

            //For efficiency, we just use a 32-bit exponent
            exponent = 0;
            mantissa = new BigInt(new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
            //scratch = new BigInt(new PrecisionSpec(newManBits, PrecisionSpec.BaseType.BIN));
        }

        //************************** Properties *************************

        /// <summary>
        /// Read-only property. Returns the precision specification of the mantissa.
        /// 
        /// Floating point numbers are represented as 2^exponent * mantissa, where the
        /// mantissa and exponent are integers. Note that the exponent in this class is
        /// always a 32-bit integer. The precision therefore specifies how many bits
        /// the mantissa will have.
        /// </summary>
        public PrecisionSpec Precision
        {
            get { return mantissa.Precision; }
        }

        /// <summary>
        /// Writable property:
        ///     true iff the number is negative or in some cases zero (&lt;0)
        ///     false iff the number if positive or in some cases zero (&gt;0)
        /// </summary>
        public bool Sign 
        { 
            get { return mantissa.Sign; }
            set { mantissa.Sign = value; }
        }

        /// <summary>
        /// Read-only property. 
        /// True if the number is NAN, INF_PLUS, INF_MINUS or ZERO
        /// False if the number has any other value.
        /// </summary>
        public bool IsSpecialValue
        {
            get
            {
                return (exponent == Int32.MaxValue || mantissa.IsZero());
            }
        }

        /// <summary>
        /// Read-only property, returns the type of number this is. Special values include:
        /// 
        /// NONE - a regular number
        /// ZERO - zero
        /// NAN - Not a Number (some operations will return this if their inputs are out of range)
        /// INF_PLUS - Positive infinity, not really a number, but a valid input to and output of some functions.
        /// INF_MINUS - Negative infinity, not really a number, but a valid input to and output of some functions.
        /// </summary>
        public SpecialValueType SpecialValue
        {
            get
            {
                if (exponent == Int32.MaxValue)
                {
                    if (mantissa.IsZero())
                    {
                        if (mantissa.Sign) return SpecialValueType.INF_MINUS;
                        return SpecialValueType.INF_PLUS;
                    }

                    return SpecialValueType.NAN;
                }
                else
                {
                    if (mantissa.IsZero()) return SpecialValueType.ZERO;
                    return SpecialValueType.NONE;
                }
            }
        }

        //******************** Mathematical Constants *******************

        /// <summary>
        /// Gets pi to the indicated precision
        /// </summary>
        /// <param name="precision">The precision to perform the calculation to</param>
        /// <returns>pi (the ratio of the area of a circle to its diameter)</returns>
        public static BigFloat GetPi(PrecisionSpec precision)
        {
            if (pi == null || precision.NumBits <= pi.mantissa.Precision.NumBits)
            {
                CalculatePi(precision.NumBits);
            }

            BigFloat ret = new BigFloat (precision);
            ret.Assign(pi);

            return ret;
        }

        /// <summary>
        /// Get e to the indicated precision
        /// </summary>
        /// <param name="precision">The preicision to perform the calculation to</param>
        /// <returns>e (the number for which the d/dx(e^x) = e^x)</returns>
        public static BigFloat GetE(PrecisionSpec precision)
        {
            if (eCache == null || eCache.mantissa.Precision.NumBits < precision.NumBits)
            {
                CalculateEOnly(precision.NumBits);
                //CalculateFactorials(precision.NumBits);
            }

            BigFloat ret = new BigFloat(precision);
            ret.Assign(eCache);

            return ret;
        }


        //******************** Arithmetic Functions ********************

        /// <summary>
        /// Addition (this = this + n2)
        /// </summary>
        /// <param name="n2">The number to add</param>
        public void Add(BigFloat n2)
        {
            if (SpecialValueAddTest(n2)) return;

            if (scratch.Precision.NumBits != n2.mantissa.Precision.NumBits)
            {
                scratch = new BigInt(n2.mantissa.Precision);
            }

            if (exponent <= n2.exponent)
            {
                int diff = n2.exponent - exponent;
                exponent = n2.exponent;

                if (diff != 0)
                {
                    mantissa.RSH(diff);
                }

                uint carry = mantissa.Add(n2.mantissa);

                if (carry != 0)
                {
                    mantissa.RSH(1);
                    mantissa.SetBit(mantissa.Precision.NumBits - 1);
                    exponent++;
                }

                exponent -= mantissa.Normalise();
            }
            else
            {
                int diff = exponent - n2.exponent;

                scratch.Assign(n2.mantissa);
                scratch.RSH(diff);

                uint carry = scratch.Add(mantissa);

                if (carry != 0)
                {
                    scratch.RSH(1);
                    scratch.SetBit(mantissa.Precision.NumBits - 1);
                    exponent++;
                }

                mantissa.Assign(scratch);

                exponent -= mantissa.Normalise();
            }
        }

        /// <summary>
        /// Subtraction (this = this - n2)
        /// </summary>
        /// <param name="n2">The number to subtract from this</param>
        public void Sub(BigFloat n2)
        {
            n2.mantissa.Sign = !n2.mantissa.Sign;
            Add(n2);
            n2.mantissa.Sign = !n2.mantissa.Sign;
        }

        /// <summary>
        /// Multiplication (this = this * n2)
        /// </summary>
        /// <param name="n2">The number to multiply this by</param>
        public void Mul(BigFloat n2)
        {
            if (SpecialValueMulTest(n2)) return;

            //Anything times 0 = 0
            if (n2.mantissa.IsZero())
            {
                mantissa.Assign(n2.mantissa);
                exponent = 0;
                return;
            }

            mantissa.MulHi(n2.mantissa);
            int shift = mantissa.Normalise();
            exponent = exponent + n2.exponent + 1 - shift;
        }

        /// <summary>
        /// Division (this = this / n2)
        /// </summary>
        /// <param name="n2">The number to divide this by</param>
        public void Div(BigFloat n2)
        {
            if (SpecialValueDivTest(n2)) return;

            if (mantissa.Precision.NumBits >= 8192)
            {
                BigFloat rcp = n2.Reciprocal();
                Mul(rcp);
            }
            else
            {
                int shift = mantissa.DivAndShift(n2.mantissa);
                exponent = exponent - (n2.exponent + shift);
            }
        }

        /// <summary>
        /// Multiply by a power of 2 (-ve implies division)
        /// </summary>
        /// <param name="pow2"></param>
        public void MulPow2(int pow2)
        {
            exponent += pow2;
        }

        /// <summary>
        /// Division-based reciprocal, fastest for small precisions up to 15,000 bits.
        /// </summary>
        /// <returns>The reciprocal 1/this</returns>
        public BigFloat Reciprocal()
        {
            if (mantissa.Precision.NumBits >= 8192) return ReciprocalNewton();

            BigFloat reciprocal = new BigFloat(1u, mantissa.Precision);
            reciprocal.Div(this);
            return reciprocal;
        }

        /// <summary>
        /// Newton's method reciprocal, fastest for larger precisions over 15,000 bits.
        /// </summary>
        /// <returns>The reciprocal 1/this</returns>
        public BigFloat ReciprocalNewton()
        {
            if (mantissa.IsZero())
            {
                exponent = Int32.MaxValue;
                return null;
            }

            bool oldSign = mantissa.Sign;
            int oldExponent = exponent;

            //Kill exponent for now (will re-institute later)
            exponent = 0;

            bool topBit = mantissa.IsTopBitOnlyBit();

            PrecisionSpec curPrec = new PrecisionSpec(32, PrecisionSpec.BaseType.BIN);

            BigFloat reciprocal = new BigFloat(curPrec);
            BigFloat constant2 = new BigFloat(curPrec);
            BigFloat temp = new BigFloat(curPrec);
            BigFloat thisPrec = new BigFloat(this, curPrec);

            reciprocal.exponent = 1;
            reciprocal.mantissa.SetHighDigit(3129112985u);

            constant2.exponent = 1;
            constant2.mantissa.SetHighDigit(0x80000000u);

            //D is deliberately left negative for all the following operations.
            thisPrec.mantissa.Sign = true;

            //Initial estimate.
            reciprocal.Add(thisPrec);

            //mantissa.Sign = false;

            //Shift down into 0.5 < this < 1 range
            thisPrec.mantissa.RSH(1);

            //Iteration.
            int accuracyBits = 2;
            int mantissaBits = mantissa.Precision.NumBits;

            //Each iteration is a pass of newton's method for RCP.
            //The is a substantial optimisation to be done here...
            //You can double the number of bits for the calculations
            //at each iteration, meaning that the whole process only
            //takes some constant multiplier of the time for the
            //full-scale multiplication.
            while (accuracyBits < mantissaBits)
            {
                //Increase the precision as needed
                if (accuracyBits >= curPrec.NumBits / 2)
                {
                    int newBits = curPrec.NumBits * 2;
                    if (newBits > mantissaBits) newBits = mantissaBits;
                    curPrec = new PrecisionSpec(newBits, PrecisionSpec.BaseType.BIN);

                    reciprocal = new BigFloat(reciprocal, curPrec);

                    constant2 = new BigFloat(curPrec);
                    constant2.exponent = 1;
                    constant2.mantissa.SetHighDigit(0x80000000u);

                    temp = new BigFloat(temp, curPrec);

                    thisPrec = new BigFloat(this, curPrec);
                    thisPrec.mantissa.Sign = true;
                    thisPrec.mantissa.RSH(1);
                }

                //temp = Xn
                temp.exponent = reciprocal.exponent;
                temp.mantissa.Assign(reciprocal.mantissa);
                //temp = -Xn * D
                temp.Mul(thisPrec);
                //temp = -Xn * D + 2 (= 2 - Xn * D)
                temp.Add(constant2);
                //reciprocal = X(n+1) = Xn * (2 - Xn * D)
                reciprocal.Mul(temp);

                accuracyBits *= 2;
            }

            //'reciprocal' is now the reciprocal of the shifted down, zero-exponent mantissa of 'this'
            //Restore the mantissa.
            //mantissa.LSH(1);
            exponent = oldExponent;
            //mantissa.Sign = oldSign;

            if (topBit)
            {
                reciprocal.exponent = -(oldExponent);
            }
            else
            {
                reciprocal.exponent = -(oldExponent + 1);
            }
            reciprocal.mantissa.Sign = oldSign;

            return reciprocal;
        }

        /// <summary>
        /// Newton's method reciprocal, fastest for larger precisions over 15,000 bits.
        /// </summary>
        /// <returns>The reciprocal 1/this</returns>
        private BigFloat ReciprocalNewton2()
        {
            if (mantissa.IsZero())
            {
                exponent = Int32.MaxValue;
                return null;
            }

            bool oldSign = mantissa.Sign;
            int oldExponent = exponent;

            //Kill exponent for now (will re-institute later)
            exponent = 0;

            BigFloat reciprocal = new BigFloat(mantissa.Precision);
            BigFloat constant2 = new BigFloat(mantissa.Precision);
            BigFloat temp = new BigFloat(mantissa.Precision);

            reciprocal.exponent = 1;
            reciprocal.mantissa.SetHighDigit(3129112985u);

            constant2.exponent = 1;
            constant2.mantissa.SetHighDigit(0x80000000u);

            //D is deliberately left negative for all the following operations.
            mantissa.Sign = true;

            //Initial estimate.
            reciprocal.Add(this);

            //mantissa.Sign = false;

            //Shift down into 0.5 < this < 1 range
            mantissa.RSH(1);
            
            //Iteration.
            int accuracyBits = 2;
            int mantissaBits = mantissa.Precision.NumBits;

            //Each iteration is a pass of newton's method for RCP.
            //The is a substantial optimisation to be done here...
            //You can double the number of bits for the calculations
            //at each iteration, meaning that the whole process only
            //takes some constant multiplier of the time for the
            //full-scale multiplication.
            while (accuracyBits < mantissaBits)
            {
                //temp = Xn
                temp.exponent = reciprocal.exponent;
                temp.mantissa.Assign(reciprocal.mantissa);
                //temp = -Xn * D
                temp.Mul(this);
                //temp = -Xn * D + 2 (= 2 - Xn * D)
                temp.Add(constant2);
                //reciprocal = X(n+1) = Xn * (2 - Xn * D)
                reciprocal.Mul(temp);

                accuracyBits *= 2;
            }

            //'reciprocal' is now the reciprocal of the shifted down, zero-exponent mantissa of 'this'
            //Restore the mantissa.
            mantissa.LSH(1);
            exponent = oldExponent;
            mantissa.Sign = oldSign;

            reciprocal.exponent = -(oldExponent + 1);
            reciprocal.mantissa.Sign = oldSign;

            return reciprocal;
        }

        /// <summary>
        /// Sets this equal to the input
        /// </summary>
        /// <param name="n2"></param>
        public void Assign(BigFloat n2)
        {
            exponent = n2.exponent;
            if (mantissa.AssignHigh(n2.mantissa)) exponent++;
        }


        //********************* Comparison Functions *******************

        /// <summary>
        /// Greater than comparison
        /// </summary>
        /// <param name="n2">the number to compare this to</param>
        /// <returns>true iff this is greater than n2 (this &gt; n2)</returns>
        public bool GreaterThan(BigFloat n2)
        {
            if (IsSpecialValue || n2.IsSpecialValue)
            {
                SpecialValueType s1 = SpecialValue;
                SpecialValueType s2 = SpecialValue;

                if (s1 == SpecialValueType.NAN || s2 == SpecialValueType.NAN) return false;
                if (s1 == SpecialValueType.INF_MINUS) return false;
                if (s2 == SpecialValueType.INF_PLUS) return false;
                if (s1 == SpecialValueType.INF_PLUS) return true;
                if (s2 == SpecialValueType.INF_MINUS) return true;

                if (s1 == SpecialValueType.ZERO)
                {
                    if (s2 != SpecialValueType.ZERO && n2.Sign)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (s2 == SpecialValueType.ZERO)
                {
                    return !Sign;
                }
            }

            if (!mantissa.Sign && n2.mantissa.Sign) return true;
            if (mantissa.Sign && !n2.mantissa.Sign) return false;
            if (!mantissa.Sign)
            {
                if (exponent > n2.exponent) return true;
                if (exponent < n2.exponent) return false;
            }
            if (mantissa.Sign)
            {
                if (exponent > n2.exponent) return false;
                if (exponent < n2.exponent) return true;
            }

            return mantissa.GreaterThan(n2.mantissa);
        }

        /// <summary>
        /// Less than comparison
        /// </summary>
        /// <param name="n2">the number to compare this to</param>
        /// <returns>true iff this is less than n2 (this &lt; n2)</returns>
        public bool LessThan(BigFloat n2)
        {
            if (IsSpecialValue || n2.IsSpecialValue)
            {
                SpecialValueType s1 = SpecialValue;
                SpecialValueType s2 = SpecialValue;

                if (s1 == SpecialValueType.NAN || s2 == SpecialValueType.NAN) return false;
                if (s1 == SpecialValueType.INF_PLUS) return false;
                if (s2 == SpecialValueType.INF_PLUS) return true;
                if (s2 == SpecialValueType.INF_MINUS) return false;
                if (s1 == SpecialValueType.INF_MINUS) return true;

                if (s1 == SpecialValueType.ZERO)
                {
                    if (s2 != SpecialValueType.ZERO && !n2.Sign)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (s2 == SpecialValueType.ZERO)
                {
                    return Sign;
                }
            }

            if (!mantissa.Sign && n2.mantissa.Sign) return false;
            if (mantissa.Sign && !n2.mantissa.Sign) return true;
            if (!mantissa.Sign)
            {
                if (exponent > n2.exponent) return false;
                if (exponent < n2.exponent) return true;
            }
            if (mantissa.Sign)
            {
                if (exponent > n2.exponent) return true;
                if (exponent < n2.exponent) return false;
            }

            return mantissa.LessThan(n2.mantissa);
        }

        /// <summary>
        /// Greater than comparison
        /// </summary>
        /// <param name="i">the number to compare this to</param>
        /// <returns>true iff this is greater than n2 (this &gt; n2)</returns>
        public bool GreaterThan(int i)
        {
            BigFloat integer = new BigFloat(i, mantissa.Precision);
            return GreaterThan(integer);
        }

        /// <summary>
        /// Less than comparison
        /// </summary>
        /// <param name="i">the number to compare this to</param>
        /// <returns>true iff this is less than n2 (this &lt; n2)</returns>
        public bool LessThan(int i)
        {
            BigFloat integer = new BigFloat(i, mantissa.Precision);
            return LessThan(integer);
        }

        /// <summary>
        /// Compare to zero
        /// </summary>
        /// <returns>true if this is zero (this == 0)</returns>
        public bool IsZero()
        {
            return (mantissa.IsZero());
        }


        //******************** Mathematical Functions ******************

        /// <summary>
        /// Sets the number to the biggest integer numerically closer to zero, if possible.
        /// </summary>
        public void Floor()
        {
            //Already an integer.
            if (exponent >= mantissa.Precision.NumBits) return;

            if (exponent < 0)
            {
                mantissa.ZeroBits(mantissa.Precision.NumBits);
                exponent = 0;
                return;
            }

            mantissa.ZeroBits(mantissa.Precision.NumBits - (exponent + 1));
        }

        /// <summary>
        /// Sets the number to its fractional component (equivalent to 'this' - (int)'this')
        /// </summary>
        public void FPart()
        {
            //Already fractional
            if (exponent < 0)
            {
                return;
            }

            //Has no fractional part
            if (exponent >= mantissa.Precision.NumBits)
            {
                mantissa.Zero();
                exponent = 0;
                return;
            }

            mantissa.ZeroBitsHigh(exponent + 1);
            exponent -= mantissa.Normalise();
        }

        /// <summary>
        /// Calculates tan(x)
        /// </summary>
        public void Tan()
        {
            if (IsSpecialValue)
            {
                //Tan(x) has no limit as x->inf
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS)
                {
                    SetNaN();
                }
                else if (SpecialValue == SpecialValueType.ZERO)
                {
                    SetZero();
                }

                return;
            }

            if (pi == null || pi.mantissa.Precision.NumBits != mantissa.Precision.NumBits)
            {
                CalculatePi(mantissa.Precision.NumBits);
            }

            //Work out the sign change (involves replicating some rescaling).
            bool sign = mantissa.Sign;
            mantissa.Sign = false;

            if (mantissa.IsZero())
            {
                return;
            }

            //Rescale into 0 <= x < pi
            if (GreaterThan(pi))
            {
                //There will be an inherent loss of precision doing this.
                BigFloat newAngle = new BigFloat(this);
                newAngle.Mul(piRecip);
                newAngle.FPart();
                newAngle.Mul(pi);
                Assign(newAngle);
            }

            //Rescale to -pi/2 <= x < pi/2
            if (!LessThan(piBy2))
            {
                Sub(pi);
            }

            //Now the sign of the sin determines the sign of the tan.
            //tan(x) = sin(x) / sqrt(1 - sin^2(x))
            Sin();
            BigFloat denom = new BigFloat(this);
            denom.Mul(this);
            denom.Sub(new BigFloat(1, mantissa.Precision));
            denom.mantissa.Sign = !denom.mantissa.Sign;

            if (denom.mantissa.Sign)
            {
                denom.SetZero();
            }

            denom.Sqrt();
            Div(denom);
            if (sign) mantissa.Sign = !mantissa.Sign;
        }

        /// <summary>
        /// Calculates Cos(x)
        /// </summary>
        public void Cos()
        {
            if (IsSpecialValue)
            {
                //Cos(x) has no limit as x->inf
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS)
                {
                    SetNaN();
                }
                else if (SpecialValue == SpecialValueType.ZERO)
                {
                    Assign(new BigFloat(1, mantissa.Precision));
                }

                return;
            }

            if (pi == null || pi.mantissa.Precision.NumBits != mantissa.Precision.NumBits)
            {
                CalculatePi(mantissa.Precision.NumBits);
            }

            Add(piBy2);
            Sin();
        }

        /// <summary>
        /// Calculates Sin(x):
        /// This takes a little longer and is less accurate if the input is out of the range (-pi, pi].
        /// </summary>
        public void Sin()
        {
            if (IsSpecialValue)
            {
                //Sin(x) has no limit as x->inf
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS)
                {
                    SetNaN();
                }

                return;
            }

            //Convert to positive range (0 <= x < inf)
            bool sign = mantissa.Sign;
            mantissa.Sign = false;

            if (pi == null || pi.mantissa.Precision.NumBits != mantissa.Precision.NumBits)
            {
                CalculatePi(mantissa.Precision.NumBits);
            }

            if (inverseFactorialCache == null || invFactorialCutoff != mantissa.Precision.NumBits)
            {
                CalculateFactorials(mantissa.Precision.NumBits);
            }

            //Rescale into 0 <= x < 2*pi
            if (GreaterThan(twoPi))
            {
                //There will be an inherent loss of precision doing this.
                BigFloat newAngle = new BigFloat(this);
                newAngle.Mul(twoPiRecip);
                newAngle.FPart();
                newAngle.Mul(twoPi);
                Assign(newAngle);
            }

            //Rescale into range 0 <= x < pi
            if (GreaterThan(pi))
            {
                //sin(pi + a) = sin(pi)cos(a) + sin(a)cos(pi) = 0 - sin(a) = -sin(a)
                Sub(pi);
                sign = !sign;
            }

            BigFloat temp = new BigFloat(mantissa.Precision);

            //Rescale into range 0 <= x < pi/2
            if (GreaterThan(piBy2))
            {
                temp.Assign(this);
                Assign(pi);
                Sub(temp);
            }

            //Rescale into range 0 <= x < pi/6 to accelerate convergence.
            //This is done using sin(3x) = 3sin(x) - 4sin^3(x)
            Mul(threeRecip);

            if (mantissa.IsZero())
            {
                exponent = 0;
                return;
            }

            BigFloat term = new BigFloat(this);

            BigFloat square = new BigFloat(this);
            square.Mul(term);

            BigFloat sum = new BigFloat(this);

            bool termSign = true;
            int length = inverseFactorialCache.Length;
            int numBits = mantissa.Precision.NumBits;

            for (int i = 3; i < length; i += 2)
            {
                term.Mul(square);
                temp.Assign(inverseFactorialCache[i]);
                temp.Mul(term);
                temp.mantissa.Sign = termSign;
                termSign = !termSign;

                if (temp.exponent < -numBits) break;

                sum.Add(temp);
            }

            //Restore the triple-angle: sin(3x) = 3sin(x) - 4sin^3(x)
            Assign(sum);
            sum.Mul(this);
            sum.Mul(this);
            Mul(new BigFloat(3, mantissa.Precision));
            sum.exponent += 2;
            Sub(sum);

            //Restore the sign
            mantissa.Sign = sign;
        }

        /// <summary>
        /// Hyperbolic Sin (sinh) function
        /// </summary>
        public void Sinh()
        {
            if (IsSpecialValue)
            {
                return;
            }

            Exp();
            Sub(Reciprocal());
            exponent--;
        }

        /// <summary>
        /// Hyperbolic cosine (cosh) function
        /// </summary>
        public void Cosh()
        {
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.ZERO)
                {
                    Assign(new BigFloat(1, mantissa.Precision));
                }
                else if (SpecialValue == SpecialValueType.INF_MINUS)
                {
                    SetInfPlus();
                }

                return;
            }

            Exp();
            Add(Reciprocal());
            exponent--;
        }

        /// <summary>
        /// Hyperbolic tangent function (tanh)
        /// </summary>
        public void Tanh()
        {
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS)
                {
                    Assign(new BigFloat(-1, mantissa.Precision));
                }
                else if (SpecialValue == SpecialValueType.INF_PLUS)
                {
                    Assign(new BigFloat(1, mantissa.Precision));
                }

                return;
            }

            exponent++;
            Exp();
            BigFloat temp = new BigFloat(this);
            BigFloat one = new BigFloat(1, mantissa.Precision);
            temp.Add(one);
            Sub(one);
            Div(temp);
        }

        /// <summary>
        /// arcsin(): the inverse function of sin(), range of (-pi/2..pi/2)
        /// </summary>
        public void Arcsin()
        {
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS || SpecialValue == SpecialValueType.NAN)
                {
                    SetNaN();
                    return;
                }

                return;
            }

            BigFloat one = new BigFloat(1, mantissa.Precision);
            BigFloat plusABit = new BigFloat(1, mantissa.Precision);
            plusABit.exponent -= (mantissa.Precision.NumBits - (mantissa.Precision.NumBits >> 6));
            BigFloat onePlusABit = new BigFloat(1, mantissa.Precision);
            onePlusABit.Add(plusABit);

            bool sign = mantissa.Sign;
            mantissa.Sign = false;

            if (GreaterThan(onePlusABit))
            {
                SetNaN();
            }
            else if (LessThan(one))
            {
                BigFloat temp = new BigFloat(this);
                temp.Mul(this);
                temp.Sub(one);
                temp.mantissa.Sign = !temp.mantissa.Sign;
                temp.Sqrt();
                temp.Add(one);
                Div(temp);
                Arctan();
                exponent++;
                mantissa.Sign = sign;
            }
            else
            {
                if (pi == null || pi.mantissa.Precision.NumBits != mantissa.Precision.NumBits)
                {
                    CalculatePi(mantissa.Precision.NumBits);
                }

                Assign(piBy2);
                if (sign) mantissa.Sign = true;
            }
        }

        /// <summary>
        /// arccos(): the inverse function of cos(), range (0..pi)
        /// </summary>
        public void Arccos()
        {
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS || SpecialValue == SpecialValueType.NAN)
                {
                    SetNaN();
                }
                else if (SpecialValue == SpecialValueType.ZERO)
                {
                    Assign(new BigFloat(1, mantissa.Precision));
                    exponent = 0;
                    Sign = false;
                }

                return;
            }

            BigFloat one = new BigFloat(1, mantissa.Precision);
            BigFloat plusABit = new BigFloat(1, mantissa.Precision);
            plusABit.exponent -= (mantissa.Precision.NumBits - (mantissa.Precision.NumBits >> 6));
            BigFloat onePlusABit = new BigFloat(1, mantissa.Precision);
            onePlusABit.Add(plusABit);

            bool sign = mantissa.Sign;
            mantissa.Sign = false;

            if (GreaterThan(onePlusABit))
            {
                SetNaN();
            }
            else if (LessThan(one))
            {
                if (pi == null || pi.mantissa.Precision.NumBits != mantissa.Precision.NumBits)
                {
                    CalculatePi(mantissa.Precision.NumBits);
                }

                mantissa.Sign = sign;
                BigFloat temp = new BigFloat(this);
                Mul(temp);
                Sub(one);
                mantissa.Sign = !mantissa.Sign;
                Sqrt();
                temp.Add(one);
                Div(temp);
                Arctan();
                exponent++;
            }
            else
            {
                if (sign)
                {
                    if (pi == null || pi.mantissa.Precision.NumBits != mantissa.Precision.NumBits)
                    {
                        CalculatePi(mantissa.Precision.NumBits);
                    }

                    Assign(pi);
                }
                else
                {
                    mantissa.Zero();
                    exponent = 0;
                }
            }
        }

        /// <summary>
        /// arctan(): the inverse function of sin(), range of (-pi/2..pi/2)
        /// </summary>
        public void Arctan()
        {
            //With 2 argument reductions, we increase precision by a minimum of 4 bits per term.
            int numBits = mantissa.Precision.NumBits;
            int maxTerms = numBits >> 2;

            if (pi == null || pi.mantissa.Precision.NumBits != numBits)
            {
                CalculatePi(mantissa.Precision.NumBits);
            }

            //Make domain positive
            bool sign = mantissa.Sign;
            mantissa.Sign = false;

            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS)
                {
                    Assign(piBy2);
                    mantissa.Sign = sign;
                    return;
                }

                return;
            }

            if (reciprocals == null || reciprocals[0].mantissa.Precision.NumBits != numBits || reciprocals.Length < maxTerms)
            {
                CalculateReciprocals(numBits, maxTerms);
            }

            bool invert = false;
            BigFloat one = new BigFloat(1, mantissa.Precision);

            //Invert if outside of convergence
            if (GreaterThan(one))
            {
                invert = true;
                Assign(Reciprocal());
            }

            //Reduce using half-angle formula:
            //arctan(2x) = 2 arctan (x / (1 + sqrt(1 + x)))

            //First reduction (guarantees 2 bits per iteration)
            BigFloat temp = new BigFloat(this);
            temp.Mul(this);
            temp.Add(one);
            temp.Sqrt();
            temp.Add(one);
            this.Div(temp);

            //Second reduction (guarantees 4 bits per iteration)
            temp.Assign(this);
            temp.Mul(this);
            temp.Add(one);
            temp.Sqrt();
            temp.Add(one);
            this.Div(temp);

            //Actual series calculation
            int length = reciprocals.Length;
            BigFloat term = new BigFloat(this);

            //pow = x^2
            BigFloat pow = new BigFloat(this);
            pow.Mul(this);

            BigFloat sum = new BigFloat(this);

            for (int i = 1; i < length; i++)
            {
                //u(n) = u(n-1) * x^2
                //t(n) = u(n) / (2n+1)
                term.Mul(pow);
                term.Sign = !term.Sign;
                temp.Assign(term);
                temp.Mul(reciprocals[i]);

                if (temp.exponent < -numBits) break;

                sum.Add(temp);
            }

            //Undo the reductions.
            Assign(sum);
            exponent += 2;

            if (invert)
            {
                //Assign(Reciprocal());
                mantissa.Sign = true;
                Add(piBy2);
            }

            if (sign)
            {
                mantissa.Sign = sign;
            }
        }

        /// <summary>
        /// Arcsinh(): the inverse sinh function
        /// </summary>
        public void Arcsinh()
        {
            //Just let all special values fall through
            if (IsSpecialValue)
            {
                return;
            }

            BigFloat temp = new BigFloat(this);
            temp.Mul(this);
            temp.Add(new BigFloat(1, mantissa.Precision));
            temp.Sqrt();
            Add(temp);
            Log();
        }

        /// <summary>
        /// Arccosh(): the inverse cosh() function
        /// </summary>
        public void Arccosh()
        {
            //acosh isn't defined for x < 1
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.ZERO)
                {
                    SetNaN();
                    return;
                }

                return;
            }

            BigFloat one = new BigFloat(1, mantissa.Precision);
            if (LessThan(one))
            {
                SetNaN();
                return;
            }

            BigFloat temp = new BigFloat(this);
            temp.Mul(this);
            temp.Sub(one);
            temp.Sqrt();
            Add(temp);
            Log();
        }

        /// <summary>
        /// Arctanh(): the inverse tanh function
        /// </summary>
        public void Arctanh()
        {
            //|x| <= 1 for a non-NaN output
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || SpecialValue == SpecialValueType.INF_PLUS)
                {
                    SetNaN();
                    return;
                }

                return;
            }

            BigFloat one = new BigFloat(1, mantissa.Precision);
            BigFloat plusABit = new BigFloat(1, mantissa.Precision);
            plusABit.exponent -= (mantissa.Precision.NumBits - (mantissa.Precision.NumBits >> 6));
            BigFloat onePlusABit = new BigFloat(1, mantissa.Precision);
            onePlusABit.Add(plusABit);

            bool sign = mantissa.Sign;
            mantissa.Sign = false;

            if (GreaterThan(onePlusABit))
            {
                SetNaN();
            }
            else if (LessThan(one))
            {
                BigFloat temp = new BigFloat(this);
                Add(one);
                one.Sub(temp);
                Div(one);
                Log();
                exponent--;
                mantissa.Sign = sign;
            }
            else
            {
                if (sign)
                {
                    SetInfMinus();
                }
                else
                {
                    SetInfPlus();
                }
            }
        }

        /// <summary>
        /// Two-variable iterative square root, taken from
        /// http://en.wikipedia.org/wiki/Methods_of_computing_square_roots#A_two-variable_iterative_method
        /// </summary>
        public void Sqrt()
        {
            if (mantissa.Sign || IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.ZERO)
                {
                    return;
                }

                if (SpecialValue == SpecialValueType.INF_MINUS || mantissa.Sign)
                {
                    SetNaN();
                }

                return;
            }

            BigFloat temp2;
            BigFloat temp3 = new BigFloat(mantissa.Precision);
            BigFloat three = new BigFloat(3, mantissa.Precision);

            int exponentScale = 0;

            //Rescale to 0.5 <= x < 2
            if (exponent < -1)
            {
                int diff = -exponent;
                if ((diff & 1) != 0)
                {
                    diff--;
                }

                exponentScale = -diff;
                exponent += diff;
            }
            else if (exponent > 0)
            {
                if ((exponent & 1) != 0)
                {
                    exponentScale = exponent + 1;
                    exponent = -1;
                }
                else
                {
                    exponentScale = exponent;
                    exponent = 0;
                }
            }

            temp2 = new BigFloat(this);
            temp2.Sub(new BigFloat(1, mantissa.Precision));

            //if (temp2.mantissa.IsZero())
            //{
            //    exponent += exponentScale;
            //    return;
            //}

            int numBits = mantissa.Precision.NumBits;

            while ((exponent - temp2.exponent) < numBits && temp2.SpecialValue != SpecialValueType.ZERO)
            {
                //a(n+1) = an - an*cn / 2
                temp3.Assign(this);
                temp3.Mul(temp2);
                temp3.MulPow2(-1);
                this.Sub(temp3);

                //c(n+1) = cn^2 * (cn - 3) / 4
                temp3.Assign(temp2);
                temp2.Sub(three);
                temp2.Mul(temp3);
                temp2.Mul(temp3);
                temp2.MulPow2(-2);
            }

            exponent += (exponentScale >> 1);
        }

        /// <summary>
        /// The natural logarithm, ln(x)
        /// </summary>
        public void Log()
        {
            if (IsSpecialValue || mantissa.Sign)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || mantissa.Sign)
                {
                    SetNaN();
                }
                else if (SpecialValue == SpecialValueType.ZERO)
                {
                    SetInfMinus();
                }

                return;
            }

            if (mantissa.Precision.NumBits >= 512)
            {
                LogAGM1();
                return;
            }

            //Compute ln2.
            if (ln2cache == null || mantissa.Precision.NumBits > ln2cache.mantissa.Precision.NumBits)
            {
                CalculateLog2(mantissa.Precision.NumBits);
            }

            Log2();
            Mul(ln2cache);
        }

        /// <summary>
        /// Log to the base 10
        /// </summary>
        public void Log10()
        {
            if (IsSpecialValue || mantissa.Sign)
            {
                if (SpecialValue == SpecialValueType.INF_MINUS || mantissa.Sign)
                {
                    SetNaN();
                }
                else if (SpecialValue == SpecialValueType.ZERO)
                {
                    SetInfMinus();
                }

                return;
            }

            //Compute ln2.
            if (ln2cache == null || mantissa.Precision.NumBits > ln2cache.mantissa.Precision.NumBits)
            {
                CalculateLog2(mantissa.Precision.NumBits);
            }

            Log();
            Mul(log10recip);
        }

        /// <summary>
        /// The exponential function. Less accurate for high exponents, scales poorly with the number
        /// of bits.
        /// </summary>
        public void Exp()
        {
            Exp(mantissa.Precision.NumBits);
        }

        /// <summary>
        /// Raises a number to an integer power (positive or negative). This is a very accurate and fast function,
        /// comparable to or faster than division (although it is slightly slower for
        /// negative powers, obviously)
        /// 
        /// </summary>
        /// <param name="power"></param>
        public void Pow(int power)
        {
            BigFloat acc = new BigFloat(1, mantissa.Precision);
            BigFloat temp = new BigFloat(1, mantissa.Precision);

            int powerTemp = power;

            if (power < 0)
            {
                Assign(Reciprocal());
                powerTemp = -power;
            }

            //Fast power function
            while (powerTemp != 0)
            {
                temp.Mul(this);
                Assign(temp);

                if ((powerTemp & 1) != 0)
                {
                    acc.Mul(temp);
                }

                powerTemp >>= 1;
            }

            Assign(acc);
        }

        /// <summary>
        /// Raises to an aribitrary power. This is both slow (uses Log) and inaccurate. If you need to
        /// raise e^x use exp(). If you need an integer power, use the integer power function Pow(int)
        /// Accuracy Note:
        ///    The function is only ever accurate to a maximum of 4 decimal digits
        ///    For every 10x larger (or smaller) the power gets, you lose an additional decimal digit
        ///    If you really need a precise result, do the calculation with an extra 32-bits and round
        /// Domain Note:
        ///    This only works for powers of positive real numbers. Negative numbers will fail.
        /// </summary>
        /// <param name="power"></param>
        public void Pow(BigFloat power)
        {
            Log();
            Mul(power);
            Exp();
        }


        //******************** Static Math Functions *******************

        /// <summary>
        /// Returns the integer component of the input
        /// </summary>
        /// <param name="n1">The input number</param>
        /// <remarks>The integer component returned will always be numerically closer to zero
        /// than the input: an input of -3.49 for instance would produce a value of 3.</remarks>
        public static BigFloat Floor(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Floor();
            return n1;
        }

        /// <summary>
        /// Returns the fractional (non-integer component of the input)
        /// </summary>
        /// <param name="n1">The input number</param>
        public static BigFloat FPart(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.FPart();
            return n1;
        }

        /// <summary>
        /// Calculates tan(x)
        /// </summary>
        /// <param name="n1">The angle (in radians) to find the tangent of</param>
        public static BigFloat Tan(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Tan();
            return n1;
        }

        /// <summary>
        /// Calculates Cos(x)
        /// </summary>
        /// <param name="n1">The angle (in radians) to find the cosine of</param>
        /// <remarks>This is a reasonably fast function for smaller precisions, but
        /// doesn't scale well for higher precision arguments</remarks>
        public static BigFloat Cos(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Cos();
            return n1;
        }

        /// <summary>
        /// Calculates Sin(x):
        /// This takes a little longer and is less accurate if the input is out of the range (-pi, pi].
        /// </summary>
        /// <param name="n1">The angle to find the sine of (in radians)</param>
        /// <remarks>This is a resonably fast function, for smaller precision arguments, but doesn't
        /// scale very well with the number of bits in the input.</remarks>
        public static BigFloat Sin(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Sin();
            return n1;
        }

        /// <summary>
        /// Hyperbolic Sin (sinh) function
        /// </summary>
        /// <param name="n1">The number to find the hyperbolic sine of</param>
        public static BigFloat Sinh(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Sinh();
            return n1;
        }

        /// <summary>
        /// Hyperbolic cosine (cosh) function
        /// </summary>
        /// <param name="n1">The number to find the hyperbolic cosine of</param>
        public static BigFloat Cosh(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Cosh();
            return n1;
        }

        /// <summary>
        /// Hyperbolic tangent function (tanh)
        /// </summary>
        /// <param name="n1">The number to find the hyperbolic tangent of</param>
        public static BigFloat Tanh(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Tanh();
            return n1;
        }

        /// <summary>
        /// arcsin(): the inverse function of sin(), range of (-pi/2..pi/2)
        /// </summary>
        /// <param name="n1">The number to find the arcsine of (-pi/2..pi/2)</param>
        /// <remarks>Note that inverse trig functions are only defined within a specific range.
        /// Values outside this range will return NaN, although some margin for error is assumed.
        /// </remarks>
        public static BigFloat Arcsin(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Arcsin();
            return n1;
        }

        /// <summary>
        /// arccos(): the inverse function of cos(), input range (0..pi)
        /// </summary>
        /// <param name="n1">The number to find the arccosine of (0..pi)</param>
        /// <remarks>Note that inverse trig functions are only defined within a specific range.
        /// Values outside this range will return NaN, although some margin for error is assumed.
        /// </remarks>
        public static BigFloat Arccos(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Arccos();
            return n1;
        }

        /// <summary>
        /// arctan(): the inverse function of sin(), input range of (-pi/2..pi/2)
        /// </summary>
        /// <param name="n1">The number to find the arctangent of (-pi/2..pi/2)</param>
        /// <remarks>Note that inverse trig functions are only defined within a specific range.
        /// Values outside this range will return NaN, although some margin for error is assumed.
        /// </remarks>
        public static BigFloat Arctan(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Arctan();
            return n1;
        }

        /// <summary>
        /// Arcsinh(): the inverse sinh function
        /// </summary>
        /// <param name="n1">The number to find the inverse hyperbolic sine of</param>
        public static BigFloat Arcsinh(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Arcsinh();
            return n1;
        }

        /// <summary>
        /// Arccosh(): the inverse cosh() function
        /// </summary>
        /// <param name="n1">The number to find the inverse hyperbolic cosine of</param>
        public static BigFloat Arccosh(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Arccosh();
            return n1;
        }

        /// <summary>
        /// Arctanh(): the inverse tanh function
        /// </summary>
        /// <param name="n1">The number to fine the inverse hyperbolic tan of</param>
        public static BigFloat Arctanh(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Arctanh();
            return n1;
        }

        /// <summary>
        /// Two-variable iterative square root, taken from
        /// http://en.wikipedia.org/wiki/Methods_of_computing_square_roots#A_two-variable_iterative_method
        /// </summary>
        /// <remarks>This is quite a fast function, as elementary functions go. You can expect it to take
        /// about twice as long as a floating-point division.
        /// </remarks>
        public static BigFloat Sqrt(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Sqrt();
            return n1;
        }

        /// <summary>
        /// The natural logarithm, ln(x) (log base e)
        /// </summary>
        /// <remarks>This is a very slow function, despite repeated attempts at optimisation.
        /// To make it any faster, different strategies would be needed for integer operations.
        /// It does, however, scale well with the number of bits.
        /// </remarks>
        /// <param name="n1">The number to find the natural logarithm of</param>
        public static BigFloat Log(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Log();
            return n1;
        }

        /// <summary>
        /// Base 10 logarithm of a number
        /// </summary>
        /// <remarks>This is a very slow function, despite repeated attempts at optimisation.
        /// To make it any faster, different strategies would be needed for integer operations.
        /// It does, however, scale well with the number of bits.
        /// </remarks>
        /// <param name="n1">The number to find the base 10 logarithm of</param>
        public static BigFloat Log10(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Log10();
            return n1;
        }

        /// <summary>
        /// The exponential function. Less accurate for high exponents, scales poorly with the number
        /// of bits. This is quite fast for low-precision arguments.
        /// </summary>
        public static BigFloat Exp(BigFloat n1)
        {
            BigFloat res = new BigFloat(n1);
            n1.Exp();
            return n1;
        }

        /// <summary>
        /// Raises a number to an integer power (positive or negative). This is a very accurate and fast function,
        /// comparable to or faster than division (although it is slightly slower for
        /// negative powers, obviously).
        /// </summary>
        /// <param name="n1">The number to raise to the power</param>
        /// <param name="power">The power to raise it to</param>
        public static BigFloat Pow(BigFloat n1, int power)
        {
            BigFloat res = new BigFloat(n1);
            n1.Pow(power);
            return n1;
        }

        /// <summary>
        /// Raises to an aribitrary power. This is both slow (uses Log) and inaccurate. If you need to
        /// raise e^x use exp(). If you need an integer power, use the integer power function Pow(int)
        /// </summary>
        /// <remarks>
        /// Accuracy Note:
        ///    The function is only ever accurate to a maximum of 4 decimal digits
        ///    For every 10x larger (or smaller) the power gets, you lose an additional decimal digit
        ///    If you really need a precise result, do the calculation with an extra 32-bits and round
        ///    
        /// Domain Note:
        ///    This only works for powers of positive real numbers. Negative numbers will fail.
        /// </remarks>
        /// <param name="n1">The number to raise to a power</param>
        /// <param name="power">The power to raise it to</param>
        public static BigFloat Pow(BigFloat n1, BigFloat power)
        {
            BigFloat res = new BigFloat(n1);
            n1.Pow(power);
            return n1;
        }

        //********************** Static functions **********************

        /// <summary>
        /// Adds two numbers and returns the result
        /// </summary>
        public static BigFloat Add(BigFloat n1, BigFloat n2)
        {
            BigFloat ret = new BigFloat(n1);
            ret.Add(n2);
            return ret;
        }

        /// <summary>
        /// Subtracts two numbers and returns the result
        /// </summary>
        public static BigFloat Sub(BigFloat n1, BigFloat n2)
        {
            BigFloat ret = new BigFloat(n1);
            ret.Sub(n2);
            return ret;
        }

        /// <summary>
        /// Multiplies two numbers and returns the result
        /// </summary>
        public static BigFloat Mul(BigFloat n1, BigFloat n2)
        {
            BigFloat ret = new BigFloat(n1);
            ret.Mul(n2);
            return ret;
        }

        /// <summary>
        /// Divides two numbers and returns the result
        /// </summary>
        public static BigFloat Div(BigFloat n1, BigFloat n2)
        {
            BigFloat ret = new BigFloat(n1);
            ret.Div(n2);
            return ret;
        }

        /// <summary>
        /// Tests whether n1 is greater than n2
        /// </summary>
        public static bool GreaterThan(BigFloat n1, BigFloat n2)
        {
            return n1.GreaterThan(n2);
        }

        /// <summary>
        /// Tests whether n1 is less than n2
        /// </summary>
        public static bool LessThan(BigFloat n1, BigFloat n2)
        {
            return n1.LessThan(n2);
        }


        //******************* Fast static functions ********************

        /// <summary>
        /// Adds two numbers and assigns the result to res.
        /// </summary>
        /// <param name="res">a pre-existing BigFloat to take the result</param>
        /// <param name="n1">the first number</param>
        /// <param name="n2">the second number</param>
        /// <returns>a handle to res</returns>
        public static BigFloat Add(BigFloat res, BigFloat n1, BigFloat n2)
        {
            res.Assign(n1);
            res.Add(n2);
            return res;
        }

        /// <summary>
        /// Subtracts two numbers and assigns the result to res.
        /// </summary>
        /// <param name="res">a pre-existing BigFloat to take the result</param>
        /// <param name="n1">the first number</param>
        /// <param name="n2">the second number</param>
        /// <returns>a handle to res</returns>
        public static BigFloat Sub(BigFloat res, BigFloat n1, BigFloat n2)
        {
            res.Assign(n1);
            res.Sub(n2);
            return res;
        }

        /// <summary>
        /// Multiplies two numbers and assigns the result to res.
        /// </summary>
        /// <param name="res">a pre-existing BigFloat to take the result</param>
        /// <param name="n1">the first number</param>
        /// <param name="n2">the second number</param>
        /// <returns>a handle to res</returns>
        public static BigFloat Mul(BigFloat res, BigFloat n1, BigFloat n2)
        {
            res.Assign(n1);
            res.Mul(n2);
            return res;
        }

        /// <summary>
        /// Divides two numbers and assigns the result to res.
        /// </summary>
        /// <param name="res">a pre-existing BigFloat to take the result</param>
        /// <param name="n1">the first number</param>
        /// <param name="n2">the second number</param>
        /// <returns>a handle to res</returns>
        public static BigFloat Div(BigFloat res, BigFloat n1, BigFloat n2)
        {
            res.Assign(n1);
            res.Div(n2);
            return res;
        }


        //************************* Operators **************************

        /// <summary>
        /// The addition operator
        /// </summary>
        public static BigFloat operator +(BigFloat n1, BigFloat n2)
        {
            return Add(n1, n2);
        }

        /// <summary>
        /// The subtraction operator
        /// </summary>
        public static BigFloat operator -(BigFloat n1, BigFloat n2)
        {
            return Sub(n1, n2);
        }

        /// <summary>
        /// The multiplication operator
        /// </summary>
        public static BigFloat operator *(BigFloat n1, BigFloat n2)
        {
            return Mul(n1, n2);
        }

        /// <summary>
        /// The division operator
        /// </summary>
        public static BigFloat operator /(BigFloat n1, BigFloat n2)
        {
            return Div(n1, n2);
        }

        //************************** Conversions *************************

        /// <summary>
        /// Converts a BigFloat to an BigInt with the specified precision
        /// </summary>
        /// <param name="n1">The number to convert</param>
        /// <param name="precision">The precision to convert it with</param>
        /// <param name="round">Do we round the number if we are truncating the mantissa?</param>
        /// <returns></returns>
        public static BigInt ConvertToInt(BigFloat n1, PrecisionSpec precision, bool round)
        {
            BigInt ret = new BigInt(precision);

            int numBits = n1.mantissa.Precision.NumBits;
            int shift = numBits - (n1.exponent + 1);

            BigFloat copy = new BigFloat(n1);
            bool inc = false;

            //Rounding
            if (copy.mantissa.Precision.NumBits > ret.Precision.NumBits)
            {
                inc = true;

                for (int i = copy.exponent + 1; i <= ret.Precision.NumBits; i++)
                {
                    if (copy.mantissa.GetBitFromTop(i) == 0)
                    {
                        inc = false;
                        break;
                    }
                }
            }

            if (shift > 0)
            {
                copy.mantissa.RSH(shift);
            }
            else if (shift < 0)
            {
                copy.mantissa.LSH(-shift);
            }

            ret.Assign(copy.mantissa);

            if (inc) ret.Increment();

            return ret;
        }

        /// <summary>
        /// Returns a base-10 string representing the number.
        /// 
        /// Note: This is inefficient and possibly inaccurate. Please use with enough
        /// rounding digits (set using the RoundingDigits property) to ensure accuracy
        /// </summary>
        public override string ToString()
        {
            if (IsSpecialValue)
            {
                SpecialValueType s = SpecialValue;
                if (s == SpecialValueType.ZERO)
                {
                    return String.Format("0{0}0", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                }
                else if (s == SpecialValueType.INF_PLUS)
                {
                    return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PositiveInfinitySymbol;
                }
                else if (s == SpecialValueType.INF_MINUS)
                {
                    return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NegativeInfinitySymbol;
                }
                else if (s == SpecialValueType.NAN)
                {
                    return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NaNSymbol;
                }
                else
                {
                    return "Unrecognised special type";
                }
            }

            if (scratch.Precision.NumBits != mantissa.Precision.NumBits)
            {
                scratch = new BigInt(mantissa.Precision);
            }

            //The mantissa expresses 1.xxxxxxxxxxx
            //The highest possible value for the mantissa without the implicit 1. is 0.9999999...
            scratch.Assign(mantissa);
            //scratch.Round(3);
            scratch.Sign = false;
            BigInt denom = new BigInt("0", mantissa.Precision);
            denom.SetBit(mantissa.Precision.NumBits - 1);

            bool useExponentialNotation = false;
            int halfBits = mantissa.Precision.NumBits / 2;
            if (halfBits > 60) halfBits = 60;
            int precDec = 10;

            if (exponent > 0)
            {
                if (exponent < halfBits)
                {
                    denom.RSH(exponent);
                }
                else
                {
                    useExponentialNotation = true;
                }
            }
            else if (exponent < 0)
            {
                int shift = -(exponent);
                if (shift < precDec)
                {
                    scratch.RSH(shift);
                }
                else
                {
                    useExponentialNotation = true;
                }
            }

            string output;

            if (useExponentialNotation)
            {
                int absExponent = exponent;
                if (absExponent < 0) absExponent = -absExponent;
                int powerOf10 = (int)((double)absExponent * Math.Log10(2.0));

                //Use 1 extra digit of precision (this is actually 32 bits more, nb)
                BigFloat thisFloat = new BigFloat(this, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));
                thisFloat.mantissa.Sign = false;

                //Multiplicative correction factor to bring number into range.
                BigFloat one = new BigFloat(1, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));
                BigFloat ten = new BigFloat(10, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));
                BigFloat tenRCP = ten.Reciprocal();

                //Accumulator for the power of 10 calculation.
                BigFloat acc = new BigFloat(1, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));

                BigFloat tenToUse;

                if (exponent > 0)
                {
                    tenToUse = new BigFloat(tenRCP, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));
                }
                else
                {
                    tenToUse = new BigFloat(ten, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));
                }

                BigFloat tenToPower = new BigFloat(1, new PrecisionSpec(mantissa.Precision.NumBits + 1, PrecisionSpec.BaseType.BIN));

                int powerTemp = powerOf10;

                //Fast power function
                while (powerTemp != 0)
                {
                    tenToPower.Mul(tenToUse);
                    tenToUse.Assign(tenToPower);

                    if ((powerTemp & 1) != 0)
                    {
                        acc.Mul(tenToPower);
                    }

                    powerTemp >>= 1;
                }

                thisFloat.Mul(acc);

                //If we are out of range, correct.           
                if (thisFloat.GreaterThan(ten))
                {
                    thisFloat.Mul(tenRCP);
                    if (exponent > 0)
                    {
                        powerOf10++;
                    }
                    else
                    {
                        powerOf10--;
                    }
                }
                else if (thisFloat.LessThan(one))
                {
                    thisFloat.Mul(ten);
                    if (exponent > 0)
                    {
                        powerOf10--;
                    }
                    else
                    {
                        powerOf10++;
                    }
                }

                //Restore the precision and the sign.
                BigFloat printable = new BigFloat(thisFloat, mantissa.Precision);
                printable.mantissa.Sign = mantissa.Sign;
                output = printable.ToString();

                if (exponent < 0) powerOf10 = -powerOf10;

                output = String.Format("{0}E{1}", output, powerOf10);
            }
            else
            {
                BigInt bigDigit = BigInt.Div(scratch, denom);
                bigDigit.Sign = false;
                scratch.Sub(BigInt.Mul(denom, bigDigit));

                if (mantissa.Sign)
                {
                    output = String.Format("-{0}.", bigDigit);
                }
                else
                {
                    output = String.Format("{0}.", bigDigit);
                }

                denom = BigInt.Div(denom, 10u);

                while (!denom.IsZero())
                {
                    uint digit = (uint)BigInt.Div(scratch, denom);
                    if (digit == 10) digit--;
                    scratch.Sub(BigInt.Mul(denom, digit));
                    output = String.Format("{0}{1}", output, digit);
                    denom = BigInt.Div(denom, 10u);
                }

                output = RoundString(output, RoundingDigits);
            }

            return output;
        }

        //**************** Special value handling for ops ***************

        private void SetNaN()
        {
            exponent = Int32.MaxValue;
            mantissa.SetBit(mantissa.Precision.NumBits - 1);
        }

        private void SetZero()
        {
            exponent = 0;
            mantissa.Zero();
            Sign = false;
        }

        private void SetInfPlus()
        {
            Sign = false;
            exponent = Int32.MaxValue;
            mantissa.Zero();
        }

        private void SetInfMinus()
        {
            Sign = true;
            exponent = Int32.MaxValue;
            mantissa.Zero();
        }

        private bool SpecialValueAddTest(BigFloat n2)
        {
            if (IsSpecialValue || n2.IsSpecialValue)
            {
                SpecialValueType s1 = SpecialValue;
                SpecialValueType s2 = n2.SpecialValue;

                if (s1 == SpecialValueType.NAN) return true;
                if (s2 == SpecialValueType.NAN)
                {
                    //Set NaN and return.
                    SetNaN();
                    return true;
                }

                if (s1 == SpecialValueType.INF_PLUS)
                {
                    //INF+ + INF- = NAN
                    if (s2 == SpecialValueType.INF_MINUS)
                    {
                        SetNaN();
                        return true;
                    }

                    return true;
                }

                if (s1 == SpecialValueType.INF_MINUS)
                {
                    //INF+ + INF- = NAN
                    if (s2 == SpecialValueType.INF_PLUS)
                    {
                        SetNaN();
                        return true;
                    }

                    return true;
                }

                if (s2 == SpecialValueType.ZERO)
                {
                    return true;
                }

                if (s1 == SpecialValueType.ZERO)
                {
                    Assign(n2);
                    return true;
                }
            }

            return false;
        }

        private bool SpecialValueMulTest(BigFloat n2)
        {
            if (IsSpecialValue || n2.IsSpecialValue)
            {
                SpecialValueType s1 = SpecialValue;
                SpecialValueType s2 = n2.SpecialValue;

                if (s1 == SpecialValueType.NAN) return true;
                if (s2 == SpecialValueType.NAN)
                {
                    //Set NaN and return.
                    SetNaN();
                    return true;
                }

                if (s1 == SpecialValueType.INF_PLUS)
                {
                    //Inf+ * Inf- = Inf-
                    if (s2 == SpecialValueType.INF_MINUS)
                    {
                        Assign(n2);
                        return true;
                    }

                    //Inf+ * 0 = NaN
                    if (s2 == SpecialValueType.ZERO)
                    {
                        //Set NaN and return.
                        SetNaN();
                        return true;
                    }

                    return true;
                }

                if (s1 == SpecialValueType.INF_MINUS)
                {
                    //Inf- * Inf- = Inf+
                    if (s2 == SpecialValueType.INF_MINUS)
                    {
                        Sign = false;
                        return true;
                    }

                    //Inf- * 0 = NaN
                    if (s2 == SpecialValueType.ZERO)
                    {
                        //Set NaN and return.
                        SetNaN();
                        return true;
                    }

                    return true;
                }

                if (s2 == SpecialValueType.ZERO)
                {
                    SetZero();
                    return true;
                }

                if (s1 == SpecialValueType.ZERO)
                {
                    return true;
                }
            }

            return false;
        }

        private bool SpecialValueDivTest(BigFloat n2)
        {
            if (IsSpecialValue || n2.IsSpecialValue)
            {
                SpecialValueType s1 = SpecialValue;
                SpecialValueType s2 = n2.SpecialValue;

                if (s1 == SpecialValueType.NAN) return true;
                if (s2 == SpecialValueType.NAN)
                {
                    //Set NaN and return.
                    SetNaN();
                    return true;
                }

                if ((s1 == SpecialValueType.INF_PLUS || s1 == SpecialValueType.INF_MINUS))
                {
                    if (s2 == SpecialValueType.INF_PLUS || s2 == SpecialValueType.INF_MINUS)
                    {
                        //Set NaN and return.
                        SetNaN();
                        return true;
                    }

                    if (n2.Sign)
                    {
                        if (s1 == SpecialValueType.INF_PLUS)
                        {
                            SetInfMinus();
                            return true;
                        }

                        SetInfPlus();
                        return true;
                    }

                    //Keep inf
                    return true;
                }

                if (s2 == SpecialValueType.ZERO)
                {
                    if (s1 == SpecialValueType.ZERO)
                    {
                        SetNaN();
                        return true;
                    }

                    if (Sign)
                    {
                        SetInfMinus();
                        return true;
                    }

                    SetInfPlus();
                    return true;
                }
            }

            return false;
        }

        //****************** Internal helper functions *****************

        /// <summary>
        /// Used for fixed point speed-ups (where the extra precision is not required). Note that Denormalised
        /// floats break the assumptions that underly Add() and Sub(), so they can only be used for multiplication
        /// </summary>
        /// <param name="targetExponent"></param>
        private void Denormalise(int targetExponent)
        {
            int diff = targetExponent - exponent;
            if (diff <= 0) return;

            //This only works to reduce the precision, so if the difference implies an increase, we can't do anything.
            mantissa.RSH(diff);
            exponent += diff;
        }

        /// <summary>
        /// The binary logarithm, log2(x) - for precisions above 1000 bits, use Log() and convert the base.
        /// </summary>
        private void Log2()
        {
            if (scratch.Precision.NumBits != mantissa.Precision.NumBits)
            {
                scratch = new BigInt(mantissa.Precision);
            }

            int bits = mantissa.Precision.NumBits;
            BigFloat temp = new BigFloat(this);
            BigFloat result = new BigFloat(exponent, mantissa.Precision);
            BigFloat pow2 = new BigFloat(1, mantissa.Precision);
            temp.exponent = 0;
            int bitsCalculated = 0;

            while (bitsCalculated < bits)
            {
                int i;
                for (i = 0; (temp.exponent == 0); i++)
                {
                    temp.mantissa.SquareHiFast(scratch);
                    int shift = temp.mantissa.Normalise();
                    temp.exponent += 1 - shift;
                    if (i + bitsCalculated >= bits) break;
                }

                pow2.MulPow2(-i);
                result.Add(pow2);
                temp.exponent = 0;
                bitsCalculated += i;
            }

            this.Assign(result);
        }

        /// <summary>
        /// Tried the newton method for logs, but the exponential function is too slow to do it.
        /// </summary>
        private void LogNewton()
        {
            if (mantissa.IsZero() || mantissa.Sign)
            {
                return;
            }

            //Compute ln2.
            if (ln2cache == null || mantissa.Precision.NumBits > ln2cache.mantissa.Precision.NumBits)
            {
                CalculateLog2(mantissa.Precision.NumBits);
            }

            int numBits = mantissa.Precision.NumBits;

            //Use inverse exp function with Newton's method.
            BigFloat xn = new BigFloat(this);
            BigFloat oldExponent = new BigFloat(xn.exponent, mantissa.Precision);
            xn.exponent = 0;
            this.exponent = 0;
            //Hack to subtract 1
            xn.mantissa.ClearBit(numBits - 1);
            //x0 = (x - 1) * log2 - this is a straight line fit between log(1) = 0 and log(2) = ln2
            xn.Mul(ln2cache);
            //x0 = (x - 1) * log2 + C - this corrects for minimum error over the range.
            xn.Add(logNewtonConstant);
            BigFloat term = new BigFloat(mantissa.Precision);
            BigFloat one = new BigFloat(1, mantissa.Precision);

            int precision = 32;
            int normalPrecision = mantissa.Precision.NumBits;

            int iterations = 0;

            while (true)
            {
                term.Assign(xn);
                term.mantissa.Sign = true;
                term.Exp(precision);
                term.Mul(this);
                term.Sub(one);

                iterations++;
                if (term.exponent < -((precision >> 1) - 4))
                {
                    if (precision == normalPrecision)
                    {
                        if (term.exponent < -(precision - 4)) break;
                    }
                    else
                    {
                        precision = precision << 1;
                        if (precision > normalPrecision) precision = normalPrecision;
                    }
                }

                xn.Add(term);
            }

            //log(2^n*s) = log(2^n) + log(s) = nlog(2) + log(s)
            term.Assign(ln2cache);
            term.Mul(oldExponent);

            this.Assign(xn);
            this.Add(term);
        }

        /// <summary>
        /// Log(x) implemented as an Arithmetic-Geometric Mean. Fast for high precisions.
        /// </summary>
        private void LogAGM1()
        {
            if (mantissa.IsZero() || mantissa.Sign)
            {
                return;
            }

            //Compute ln2.
            if (ln2cache == null || mantissa.Precision.NumBits > ln2cache.mantissa.Precision.NumBits)
            {
                CalculateLog2(mantissa.Precision.NumBits);
            }

            //Compute ln(x) using AGM formula

            //1. Re-write the input as 2^n * (0.5 <= x < 1)
            int power2 = exponent + 1;
            exponent = -1;

            //BigFloat res = new BigFloat(firstAGMcache);
            BigFloat a0 = new BigFloat(1, mantissa.Precision);
            BigFloat b0 = new BigFloat(pow10cache);
            b0.Mul(this);

            BigFloat r = R(a0, b0);

            this.Assign(firstAGMcache);
            this.Sub(r);

            a0.Assign(ln2cache);
            a0.Mul(new BigFloat(power2, mantissa.Precision));
            this.Add(a0);
        }

        private void Exp(int numBits)
        {
            if (IsSpecialValue)
            {
                if (SpecialValue == SpecialValueType.ZERO)
                {
                    //e^0 = 1
                    exponent = 0;
                    mantissa.SetHighDigit(0x80000000);
                }
                else if (SpecialValue == SpecialValueType.INF_MINUS)
                {
                    //e^-inf = 0
                    SetZero();
                }

                return;
            }

            PrecisionSpec prec = new PrecisionSpec(numBits, PrecisionSpec.BaseType.BIN);
            numBits = prec.NumBits;

            if (scratch.Precision.NumBits != prec.NumBits)
            {
                scratch = new BigInt(prec);
            }

            if (inverseFactorialCache == null || invFactorialCutoff < numBits)
            {
                CalculateFactorials(numBits);
            }

            //let x = 1 * 'this'.mantissa (i.e. 1 <= x < 2)
            //exp(2^n * x) = e^(2^n * x) = (e^x)^2n = exp(x)^2n

            int oldExponent = 0;

            if (exponent > -4)
            {
                oldExponent = exponent + 4;
                exponent = -4;
            }

            BigFloat thisSave = new BigFloat(this, prec);
            BigFloat temp = new BigFloat(1, prec);
            BigFloat temp2 = new BigFloat(this, prec);
            BigFloat res = new BigFloat(1, prec);
            int length = inverseFactorialCache.Length;

            int iterations;
            for (int i = 1; i < length; i++)
            {
                //temp = x^i
                temp.Mul(thisSave);
                temp2.Assign(inverseFactorialCache[i]);
                temp2.Mul(temp);

                if (temp2.exponent < -(numBits + 4)) { iterations = i; break; }

                res.Add(temp2);
            }

            //res = exp(x)
            //Now... x^(2^n) = (x^2)^(2^(n - 1))
            for (int i = 0; i < oldExponent; i++)
            {
                res.mantissa.SquareHiFast(scratch);
                int shift = res.mantissa.Normalise();
                res.exponent = res.exponent << 1;
                res.exponent += 1 - shift;
            }

            //Deal with +/- inf
            if (res.exponent == Int32.MaxValue)
            {
                res.mantissa.Zero();
            }

            Assign(res);
        }

        /// <summary>
        /// Calculates ln(2) and returns -10^(n/2 + a bit) for reuse, using the AGM method as described in
        /// http://lacim.uqam.ca/~plouffe/articles/log2.pdf
        /// </summary>
        /// <param name="numBits"></param>
        /// <returns></returns>
        private static void CalculateLog2(int numBits)
        {
            //Use the AGM method formula to get log2 to N digits.
            //R(a0, b0) = 1 / (1 - Sum(2^-n*(an^2 - bn^2)))
            //log(1/2) = R(1, 10^-n) - R(1, 10^-n/2)
            PrecisionSpec normalPres = new PrecisionSpec(numBits, PrecisionSpec.BaseType.BIN);
            PrecisionSpec extendedPres = new PrecisionSpec(numBits + 1, PrecisionSpec.BaseType.BIN);
            BigFloat a0 = new BigFloat(1, extendedPres);
            BigFloat b0 = TenPow(-(int)((double)((numBits >> 1) + 2) * 0.302), extendedPres);
            BigFloat pow10saved = new BigFloat(b0);
            BigFloat firstAGMcacheSaved = new BigFloat(extendedPres);

            //save power of 10 (in normal precision)
            pow10cache = new BigFloat(b0, normalPres);

            ln2cache = R(a0, b0);

            //save the first half of the log calculation
            firstAGMcache = new BigFloat(ln2cache, normalPres);
            firstAGMcacheSaved.Assign(ln2cache);

            b0.MulPow2(-1);
            ln2cache.Sub(R(a0, b0));

            //Convert to log(2)
            ln2cache.mantissa.Sign = false;

            //Save magic constant for newton log
            //First guess in range 1 <= x < 2 is x0 = ln2 * (x - 1) + C
            logNewtonConstant = new BigFloat(ln2cache);
            logNewtonConstant.Mul(new BigFloat(3, extendedPres));
            logNewtonConstant.exponent--;
            logNewtonConstant.Sub(new BigFloat(1, extendedPres));
            logNewtonConstant = new BigFloat(logNewtonConstant, normalPres);

            //Save the inverse.
            log2ecache = new BigFloat(ln2cache);
            log2ecache = new BigFloat(log2ecache.Reciprocal(), normalPres);

            //Now cache log10
            //Because the log functions call this function to the precision to which they
            //are called, we cannot call them without causing an infinite loop, so we need
            //to inline the code.
            log10recip = new BigFloat(10, extendedPres);

            {
                int power2 = log10recip.exponent + 1;
                log10recip.exponent = -1;

                //BigFloat res = new BigFloat(firstAGMcache);
                BigFloat ax = new BigFloat(1, extendedPres);
                BigFloat bx = new BigFloat(pow10saved);
                bx.Mul(log10recip);

                BigFloat r = R(ax, bx);

                log10recip.Assign(firstAGMcacheSaved);
                log10recip.Sub(r);

                ax.Assign(ln2cache);
                ax.Mul(new BigFloat(power2, log10recip.mantissa.Precision));
                log10recip.Add(ax);
            }

            log10recip = log10recip.Reciprocal();
            log10recip = new BigFloat(log10recip, normalPres);


            //Trim to n bits
            ln2cache = new BigFloat(ln2cache, normalPres);
        }

        private static BigFloat TenPow(int power, PrecisionSpec precision)
        {
            BigFloat acc = new BigFloat(1, precision);
            BigFloat temp = new BigFloat(1, precision);

            int powerTemp = power;

            BigFloat multiplierToUse = new BigFloat(10, precision);

            if (power < 0)
            {
                multiplierToUse = multiplierToUse.Reciprocal();
                powerTemp = -power;
            }

            //Fast power function
            while (powerTemp != 0)
            {
                temp.Mul(multiplierToUse);
                multiplierToUse.Assign(temp);

                if ((powerTemp & 1) != 0)
                {
                    acc.Mul(temp);
                }

                powerTemp >>= 1;
            }

            return acc;
        }

        private static BigFloat R(BigFloat a0, BigFloat b0)
        {
            //Precision extend taken out.
            int bits = a0.mantissa.Precision.NumBits;
            PrecisionSpec extendedPres = new PrecisionSpec(bits, PrecisionSpec.BaseType.BIN);
            BigFloat an = new BigFloat(a0, extendedPres);
            BigFloat bn = new BigFloat(b0, extendedPres);
            BigFloat sum = new BigFloat(extendedPres);
            BigFloat term = new BigFloat(extendedPres);
            BigFloat temp1 = new BigFloat(extendedPres);
            BigFloat one = new BigFloat(1, extendedPres);

            int iteration = 0;

            for (iteration = 0; ; iteration++)
            {
                //Get the sum term for this iteration.
                term.Assign(an);
                term.Mul(an);
                temp1.Assign(bn);
                temp1.Mul(bn);
                //term = an^2 - bn^2
                term.Sub(temp1);
                //term = 2^(n-1) * (an^2 - bn^2)
                term.exponent += iteration - 1;
                sum.Add(term);

                if (term.exponent < -(bits - 8)) break;

                //Calculate the new AGM estimates.
                temp1.Assign(an);
                an.Add(bn);
                //a(n+1) = (an + bn) / 2
                an.MulPow2(-1);

                //b(n+1) = sqrt(an*bn)
                bn.Mul(temp1);
                bn.Sqrt();
            }

            one.Sub(sum);
            one = one.Reciprocal();
            return new BigFloat(one, a0.mantissa.Precision);
        }

        private static void CalculateFactorials(int numBits)
        {
            System.Collections.Generic.List<BigFloat> list = new System.Collections.Generic.List<BigFloat>(64);
            System.Collections.Generic.List<BigFloat> list2 = new System.Collections.Generic.List<BigFloat>(64);

            PrecisionSpec extendedPrecision = new PrecisionSpec(numBits + 1, PrecisionSpec.BaseType.BIN);
            PrecisionSpec normalPrecision = new PrecisionSpec(numBits, PrecisionSpec.BaseType.BIN);

            BigFloat factorial = new BigFloat(1, extendedPrecision);
            BigFloat reciprocal;

            //Calculate e while we're at it
            BigFloat e = new BigFloat(1, extendedPrecision);

            list.Add(new BigFloat(factorial, normalPrecision));

            for (int i = 1; i < Int32.MaxValue; i++)
            {
                BigFloat number = new BigFloat(i, extendedPrecision);
                factorial.Mul(number);

                if (factorial.exponent > numBits) break;

                list2.Add(new BigFloat(factorial, normalPrecision));
                reciprocal = factorial.Reciprocal();

                e.Add(reciprocal);
                list.Add(new BigFloat(reciprocal, normalPrecision));
            }

            //Set the cached static values.
            inverseFactorialCache = list.ToArray();
            factorialCache = list2.ToArray();
            invFactorialCutoff = numBits;
            eCache = new BigFloat(e, normalPrecision);
            eRCPCache = new BigFloat(e.Reciprocal(), normalPrecision);
        }

        private static void CalculateEOnly(int numBits)
        {
            PrecisionSpec extendedPrecision = new PrecisionSpec(numBits + 1, PrecisionSpec.BaseType.BIN);
            PrecisionSpec normalPrecision = new PrecisionSpec(numBits, PrecisionSpec.BaseType.BIN);

            int iExponent = (int)(Math.Sqrt(numBits));

            BigFloat factorial = new BigFloat(1, extendedPrecision);
            BigFloat constant = new BigFloat(1, extendedPrecision);
            constant.exponent -= iExponent;
            BigFloat numerator = new BigFloat(constant);
            BigFloat reciprocal;

            //Calculate the 2^iExponent th root of e
            BigFloat e = new BigFloat(1, extendedPrecision);

            int i;
            for (i = 1; i < Int32.MaxValue; i++)
            {
                BigFloat number = new BigFloat(i, extendedPrecision);
                factorial.Mul(number);
                reciprocal = factorial.Reciprocal();
                reciprocal.Mul(numerator);

                if (-reciprocal.exponent > numBits) break;

                e.Add(reciprocal);
                numerator.Mul(constant);
                System.GC.Collect();
            }

            for (i = 0; i < iExponent; i++)
            {
                numerator.Assign(e);
                e.Mul(numerator);
            }

            //Set the cached static values.
            eCache = new BigFloat(e, normalPrecision);
            eRCPCache = new BigFloat(e.Reciprocal(), normalPrecision);
        }

        /// <summary>
        /// Uses the Gauss-Legendre formula for pi
        /// Taken from http://en.wikipedia.org/wiki/Gauss%E2%80%93Legendre_algorithm
        /// </summary>
        /// <param name="numBits"></param>
        private static void CalculatePi(int numBits)
        {
            int bits = numBits + 32;
            //Precision extend taken out.
            PrecisionSpec normalPres = new PrecisionSpec(numBits, PrecisionSpec.BaseType.BIN);
            PrecisionSpec extendedPres = new PrecisionSpec(bits, PrecisionSpec.BaseType.BIN);

            if (scratch.Precision.NumBits != bits)
            {
                scratch = new BigInt(extendedPres);
            }

            //a0 = 1
            BigFloat an = new BigFloat(1, extendedPres);

            //b0 = 1/sqrt(2)
            BigFloat bn = new BigFloat(2, extendedPres);
            bn.Sqrt();
            bn.exponent--;

            //to = 1/4
            BigFloat tn = new BigFloat(1, extendedPres);
            tn.exponent -= 2;

            int pn = 0;

            BigFloat anTemp = new BigFloat(extendedPres);

            int iteration = 0;
            int cutoffBits = numBits >> 5;

            for (iteration = 0; ; iteration++)
            {
                //Save a(n)
                anTemp.Assign(an);

                //Calculate new an
                an.Add(bn);
                an.exponent--;

                //Calculate new bn
                bn.Mul(anTemp);
                bn.Sqrt();

                //Calculate new tn
                anTemp.Sub(an);
                anTemp.mantissa.SquareHiFast(scratch);
                anTemp.exponent += anTemp.exponent + pn + 1 - anTemp.mantissa.Normalise();
                tn.Sub(anTemp);

                anTemp.Assign(an);
                anTemp.Sub(bn);

                if (anTemp.exponent < -(bits - cutoffBits)) break;

                //New pn
                pn++;
            }

            an.Add(bn);
            an.mantissa.SquareHiFast(scratch);
            an.exponent += an.exponent + 1 - an.mantissa.Normalise();
            tn.exponent += 2;
            an.Div(tn);

            pi = new BigFloat(an, normalPres);
            piBy2 = new BigFloat(pi);
            piBy2.exponent--;
            twoPi = new BigFloat(pi, normalPres);
            twoPi.exponent++;
            piRecip = new BigFloat(an.Reciprocal(), normalPres);
            twoPiRecip = new BigFloat(piRecip);
            twoPiRecip.exponent--;
            //1/3 is going to be useful for sin.
            threeRecip = new BigFloat((new BigFloat(3, extendedPres)).Reciprocal(), normalPres);
        }

        /// <summary>
        /// Calculates the odd reciprocals of the natural numbers (for atan series)
        /// </summary>
        /// <param name="numBits"></param>
        /// <param name="terms"></param>
        private static void CalculateReciprocals(int numBits, int terms)
        {
            int bits = numBits + 32;
            PrecisionSpec extendedPres = new PrecisionSpec(bits, PrecisionSpec.BaseType.BIN);
            PrecisionSpec normalPres = new PrecisionSpec(numBits, PrecisionSpec.BaseType.BIN);

            System.Collections.Generic.List<BigFloat> list = new System.Collections.Generic.List<BigFloat>(terms);

            for (int i = 0; i < terms; i++)
            {
                BigFloat term = new BigFloat(i*2 + 1, extendedPres);
                list.Add(new BigFloat(term.Reciprocal(), normalPres));
            }

            reciprocals = list.ToArray();
        }

        /// <summary>
        /// Does decimal rounding, for numbers without E notation.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        private static string RoundString(string input, int places)
        {
            if (places <= 0) return input;
            string trim = input.Trim();
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

            /*
            for (int i = 1; i <= places; i++)
            {
                //Skip decimal points.
                if (trim[trim.Length - i] == '.')
                {
                    places++;
                    continue;
                }

                int index = Array.IndexOf(digits, trim[trim.Length - i]);

                if (index < 0) return input;

                value += ten * index;
                ten *= 10;
            }
             * */

            //Look for a decimal point
            string decimalPoint = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            int indexPoint = trim.LastIndexOf(decimalPoint);
            if (indexPoint < 0)
            {
                //We can't modify a string which doesn't have a decimal point.
                return trim;
            }

            int trimPoint = trim.Length - places;
            if (trimPoint < indexPoint) trimPoint = indexPoint;

            bool roundDown = false;

            if (trim[trimPoint] == '.')
            {
                if (trimPoint + 1 >= trim.Length)
                {
                    roundDown = true;
                }
                else
                {
                    int digit = Array.IndexOf(digits, trim[trimPoint + 1]);
                    if (digit < 5) roundDown = true;
                }
            }
            else
            {
                int digit = Array.IndexOf(digits, trim[trimPoint]);
                if (digit < 5) roundDown = true;
            }

            string output;

            //Round down - just return a new string without the extra digits.
            if (roundDown)
            {
                if (RoundingMode == RoundingModeType.EXACT)
                {
                    return trim.Substring(0, trimPoint);
                }
                else
                {
                    char[] trimChars = { '0' };
                    output = trim.Substring(0, trimPoint).TrimEnd(trimChars);
                    trimChars[0] = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    return output.TrimEnd(trimChars);
                }
            }
            
            //Round up - bit more complicated.
            char [] arrayOutput = trim.ToCharArray();//0, trimPoint);

            //Now, we round going from the back to the front.
            int j;
            for (j = trimPoint - 1; j >= 0; j--)
            {
                int index = Array.IndexOf(digits, arrayOutput[j]);

                //Skip decimal points etc...
                if (index < 0) continue;

                if (index < 9)
                {
                    arrayOutput[j] = digits[index + 1];
                    break;
                }
                else
                {
                    arrayOutput[j] = digits[0];
                }
            }

            output = new string(arrayOutput);

            if (j < 0)
            {
                //Need to add a new digit.
                output = String.Format("{0}{1}", "1", output);
            }

            if (RoundingMode == RoundingModeType.EXACT)
            {
                return output;
            }
            else
            {
                char[] trimChars = { '0' };
                output = output.TrimEnd(trimChars);
                trimChars[0] = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                return output.TrimEnd(trimChars);
            }
        }

        //***************************** Data *****************************


        //Side node - this way of doing things is far from optimal, both in terms of memory use and performance.
        private ExponentAdaptor exponent;
        private BigInt mantissa;

        /// <summary>
        /// Storage area for calculations.
        /// </summary>
        private static BigInt scratch;

        private static BigFloat ln2cache;                 //Value of ln(2)
        private static BigFloat log2ecache;               //Value of log2(e) = 1/ln(2)
        private static BigFloat pow10cache;               //Cached power of 10 for AGM log calculation
        private static BigFloat log10recip;               //1/ln(10)
        private static BigFloat firstAGMcache;            //Cached half of AGM operation.
        private static BigFloat[] factorialCache;         //The values of n!
        private static BigFloat[] inverseFactorialCache;  //Values of 1/n! up to 2^-m where m = invFactorialCutoff (below)
        private static int invFactorialCutoff;            //The number of significant bits for the cutoff of the inverse factorials.
        private static BigFloat eCache;                   //Value of e cached to invFactorialCutoff bits
        private static BigFloat eRCPCache;                //Reciprocal of e
        private static BigFloat logNewtonConstant;        //1.5*ln(2) - 1
        private static BigFloat pi;                       //pi
        private static BigFloat piBy2;                    //pi/2
        private static BigFloat twoPi;                    //2*pi
        private static BigFloat piRecip;                  //1/pi
        private static BigFloat twoPiRecip;               //1/2*pi
        private static BigFloat threeRecip;               //1/3
        private static BigFloat[] reciprocals;            //1/x
        
        /// <summary>
        /// The number of decimal digits to round the output of ToString() by
        /// </summary>
        public static int RoundingDigits { get; set; }

        /// <summary>
        /// The way in which ToString() should deal with insignificant trailing zeroes
        /// </summary>
        public static RoundingModeType RoundingMode { get; set; }
    }
}
