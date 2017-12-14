// http://www.fractal-landscapes.co.uk/bigint.html

using System;

namespace BigNum
{
    /// <summary>
    /// Specifies the desired precision for a BigInt or a BigFloat. 
    /// </summary>
    public struct PrecisionSpec
    {
        /// <summary>
        /// Precision can be specified in a choice of 8 bases.
        /// Note that precision for decimals is approximate.
        /// </summary>
        public enum BaseType
        {
            /// <summary>
            /// Binary base
            /// </summary>
            BIN,
            /// <summary>
            /// Octal base
            /// </summary>
            OCT,
            /// <summary>
            /// Decimal base
            /// </summary>
            DEC,
            /// <summary>
            /// Hexadecimal base
            /// </summary>
            HEX,
            /// <summary>
            /// 8-bits per digit
            /// </summary>
            BYTES,
            /// <summary>
            /// 16-bits per digit
            /// </summary>
            WORDS,
            /// <summary>
            /// 32-bits per digit
            /// </summary>
            DWORDS,
            /// <summary>
            /// 64-bits per digit
            /// </summary>
            QWORDS
        }

        /// <summary>
        /// Constructor: Constructs a precision specification
        /// </summary>
        /// <param name="precision">The number of digits</param>
        /// <param name="numberBase">The base of the digits</param>
        public PrecisionSpec(int precision, BaseType numberBase)
        {
            this.prec = precision;
            this.nB = numberBase;
        }

        /// <summary>
        /// Explicit cast from integer value.
        /// </summary>
        /// <param name="value">The value in bits for the new precision specification</param>
        /// <returns>A new precision specification with the number of bits specified</returns>
        public static explicit operator PrecisionSpec(int value)
        {
            return new PrecisionSpec(value, BaseType.BIN);
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="spec1">the first parameter</param>
        /// <param name="spec2">the second parameter</param>
        /// <returns>true iff both precisions have the same number of bits</returns>
        public static bool operator ==(PrecisionSpec spec1, PrecisionSpec spec2)
        {
            return (spec1.NumBits == spec2.NumBits);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="spec1">the first parameter</param>
        /// <param name="spec2">the second parameter</param>
        /// <returns>true iff the parameters do not have the same number of bits</returns>
        public static bool operator !=(PrecisionSpec spec1, PrecisionSpec spec2)
        {
            return !(spec1 == spec2);
        }

        /// <summary>
        /// Object equality override
        /// </summary>
        /// <param name="obj">the PrecisionSpec struct to compare</param>
        /// <returns>true iff obj has the same number of bits as this</returns>
        public override bool Equals(object obj)
        {
            return NumBits == ((PrecisionSpec)obj).NumBits;     
        }

        /// <summary>
        /// Override of the hash code
        /// </summary>
        /// <returns>A basic hash</returns>
        public override int GetHashCode()
        {
            return NumBits * prec + NumBits;
        }

        /// <summary>
        /// The precision in units specified by the base type (e.g. number of decimal digits)
        /// </summary>
        public int Precision
        {
            get { return prec; }
        }

        /// <summary>
        /// The base type in which precision is specified
        /// </summary>
        public BaseType NumberBaseType
        {
            get { return nB; }
        }

        /// <summary>
        /// Converts the number-base to an integer
        /// </summary>
        public int NumberBase
        {
            get { return (int)nB; }
        }

        /// <summary>
        /// The number of bits that this PrecisionSpec structure specifies.
        /// </summary>
        public int NumBits
        {
            get
            {
                if (nB == BaseType.BIN) return prec;
                if (nB == BaseType.OCT) return prec * 3;
                if (nB == BaseType.HEX) return prec * 4;
                if (nB == BaseType.BYTES) return prec * 8;
                if (nB == BaseType.WORDS) return prec * 16;
                if (nB == BaseType.DWORDS) return prec * 32;
                if (nB == BaseType.QWORDS) return prec * 64;

                double factor = 3.322;
                int bits = ((int)Math.Ceiling(factor * (double)prec));
                return bits;
            }
        }

        private int prec;
        private BaseType nB;
    }

    
    /// <summary>
    /// An arbitrary-precision integer class
    /// 
    /// Format:
    /// Each number consists of an array of 32-bit unsigned integers, and a bool sign
    /// value.
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
    public class BigInt
    {
        //*************** Constructors ******************

        /// <summary>
        /// Constructs an empty BigInt to the desired precision.
        /// </summary>
        /// <param name="precision"></param>
        public BigInt(PrecisionSpec precision)
        {
            Init(precision);
        }

        /// <summary>
        /// Constructs a BigInt from a string, using the string length to determine the precision
        /// Note, operations on BigInts of non-matching precision are slow, so avoid using this constructor
        /// </summary>
        /// <param name="init"></param>
        public BigInt(string init)
        {
            InitFromString(init, (PrecisionSpec)init.Length, 10);
        }

        /// <summary>
        /// Constructor for copying length and precision
        /// </summary>
        /// <param name="inputToCopy">The BigInt to copy</param>
        /// <param name="precision">The precision of the new BigInt</param>
        /// <param name="bCopyLengthOnly">decides whether to copy the actual input, or just its digit length</param>
        /// <example><code>//Create an integer
        /// BigInt four = new BigInt(4, new PrecisionSpec(128, PrecisionSpec.BaseType.BIN));
        /// 
        /// //Pad four to double its usual number of digits (this does not affect the precision)
        /// four.Pad();
        /// 
        /// //Create a new, empty integer with matching precision, also padded to twice the usual length
        /// BigInt newCopy = new BigInt(four, four.Precision, true);</code></example>
        public BigInt(BigInt inputToCopy, PrecisionSpec precision, bool bCopyLengthOnly)
        {
            digitArray = new uint[inputToCopy.digitArray.Length];
            workingSet = new uint[inputToCopy.digitArray.Length];
            if (!bCopyLengthOnly) Array.Copy(inputToCopy.digitArray, digitArray, digitArray.Length);
            sign = inputToCopy.sign;
            pres = inputToCopy.pres;
        }

        /// <summary>
        /// Constructs a bigint from the string, with the desired precision, using base 10
        /// </summary>
        /// <param name="init"></param>
        /// <param name="precision"></param>
        public BigInt(string init, PrecisionSpec precision)
        {
            InitFromString(init, precision, 10);
        }

        /// <summary>
        /// Constructs a BigInt from a string, using the specified precision and base
        /// </summary>
        /// <param name="init"></param>
        /// <param name="precision"></param>
        /// <param name="numberBase"></param>
        public BigInt(string init, PrecisionSpec precision, int numberBase)
        {
            InitFromString(init, precision, numberBase);
        }

        /// <summary>
        /// Constructs a bigint from the input.
        /// </summary>
        /// <param name="input"></param>
        public BigInt(BigInt input)
        {
            digitArray = new uint[input.digitArray.Length];
            workingSet = new uint[input.digitArray.Length];
            Array.Copy(input.digitArray, digitArray, digitArray.Length);
            sign = input.sign;
            pres = input.pres;
        }

        /// <summary>
        /// Constructs a bigint from the input, matching the new precision provided
        /// </summary>
        public BigInt(BigInt input, PrecisionSpec precision)
        {
            //Casts the input to the new precision.
            Init(precision);
            int Min = (input.digitArray.Length < digitArray.Length) ? input.digitArray.Length : digitArray.Length;

            for (int i = 0; i < Min; i++)
            {
                digitArray[i] = input.digitArray[i];
            }

            sign = input.sign;
        }

        /// <summary>
        /// Constructs a BigInt from a UInt32
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision"></param>
        public BigInt(UInt32 input, PrecisionSpec precision)
        {
            Init(precision);
            digitArray[0] = input;
        }

        /// <summary>
        /// Constructs a BigInt from a UInt64
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision"></param>
        public BigInt(UInt64 input, PrecisionSpec precision)
        {
            Init(precision);
            digitArray[0] = (UInt32)(input & 0xffffffff);
            if (digitArray.Length > 1) digitArray[1] = (UInt32)(input >> 32);
        }

        /// <summary>
        /// Constructs a BigInt from an Int32
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision"></param>
        public BigInt(Int32 input, PrecisionSpec precision)
        {
            Init(precision);
            if (input < 0)
            {
                sign = true;

                if (input == Int32.MinValue)
                {
                    digitArray[0] = 0x80000000;
                }
                else
                {
                    digitArray[0] = (UInt32)(-input);
                }
            }
            else
            {
                digitArray[0] = ((UInt32)input);
            }
        }

        /// <summary>
        /// Constructs a BigInt from a UInt32
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision"></param>
        public BigInt(Int64 input, PrecisionSpec precision)
        {
            Init(precision);
            if (input < 0) sign = true;

            digitArray[0] = (UInt32)(input & 0xffffffff);

            if (digitArray.Length >= 2)
            {
                if (input == Int64.MinValue)
                {
                    digitArray[1] = 0x80000000;
                }
                else
                {
                    digitArray[1] = (UInt32)((input >> 32) & 0x7fffffff);
                }
            }
        }

        //***************** Properties *******************

        /// <summary>
        /// true iff the number is negative
        /// </summary>
        public bool Sign { get { return sign; } set { sign = value; } }

        /// <summary>
        /// The precision of the number.
        /// </summary>
        public PrecisionSpec Precision { get { return pres; } }

        //*************** Utility Functions **************

        /// <summary>
        /// Casts a BigInt to the new precision provided.
        /// Note: This will return the input if the precision already matches.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static BigInt CastToPrecision(BigInt input, PrecisionSpec precision)
        {
            if (input.pres == precision) return input;
            return new BigInt(input, precision);
        }


        //*************** Member Functions ***************

        /// <summary>
        /// Addition and assignment - without intermediate memory allocation.
        /// </summary>
        /// <param name="n2"></param>
        /// <returns></returns>
        public uint Add(BigInt n2)
        {
            if (n2.digitArray.Length != digitArray.Length) MakeSafe(ref n2);

            if (sign == n2.sign)
            {
                return AddInternalBits(n2.digitArray);
            }
            else
            {
                bool lessThan = LtInt(this, n2);

                if (lessThan)
                {
                    int Length = digitArray.Length;

                    for (int i = 0; i < Length; i++)
                    {
                        workingSet[i] = digitArray[i];
                        digitArray[i] = n2.digitArray[i];
                    }

                    sign = !sign;
                    return SubInternalBits(workingSet);
                }
                else
                {
                    return SubInternalBits(n2.digitArray);
                }
            }
        }

        /// <summary>
        /// Subtraction and assignment - without intermediate memory allocation.
        /// </summary>
        /// <param name="n2"></param>
        public uint Sub(BigInt n2)
        {
            if (n2.digitArray.Length != digitArray.Length) MakeSafe(ref n2);

            if (sign != n2.sign)
            {
                return AddInternalBits(n2.digitArray);
            }
            else
            {
                bool lessThan = LtInt(this, n2);

                if (lessThan)
                {
                    int Length = digitArray.Length;

                    for (int i = 0; i < Length; i++)
                    {
                        workingSet[i] = digitArray[i];
                        digitArray[i] = n2.digitArray[i];
                    }

                    sign = !sign;
                    return SubInternalBits(workingSet);
                }
                else
                {
                    return SubInternalBits(n2.digitArray);
                }
            }
        }

        /// <summary>
        /// Multiplication and assignmnet - with minimal intermediate memory allocation.
        /// </summary>
        /// <param name="n2"></param>
        public void Mul(BigInt n2)
        {
            if (n2.digitArray.Length != digitArray.Length) MakeSafe(ref n2);

            int Length = n2.digitArray.Length;

            //Inner loop zero-mul avoidance
            int maxDigit = 0;
            for (int i = Length - 1; i >= 0; i--)
            {
                if (digitArray[i] != 0)
                {
                    maxDigit = i + 1;
                    break;
                }
            }

            //Result is zero, 'this' is unchanged
            if (maxDigit == 0) return;

            //Temp storage for source (both working sets are used by the calculation)
            uint[] thisTemp = new uint [Length];
            for (int i = 0; i < Length; i++)
            {
                thisTemp[i] = digitArray[i];
                digitArray[i] = 0;
            }

            for (int i = 0; i < Length; i++)
            {
                //Clear the working set
                for (int j = 0; j < i; j++)
                {
                    workingSet[j] = 0;
                    n2.workingSet[j] = 0;
                }

                n2.workingSet[i] = 0;

                ulong digit = n2.digitArray[i];
                if (digit == 0) continue;

                for (int j = 0; j + i < Length && j < maxDigit; j++)
                {
                    //Multiply n1 by each of the integer digits of n2.
                    ulong temp = (ulong)thisTemp[j] * digit;
                    //n1.workingSet stores the low bits of each piecewise multiplication
                    workingSet[j + i] = (uint)(temp & 0xffffffff);
                    if (j + i + 1 < Length)
                    {
                        //n2.workingSet stores the high bits of each multiplication
                        n2.workingSet[j + i + 1] = (uint)(temp >> 32);
                    }
                }

                AddInternalBits(workingSet);
                AddInternalBits(n2.workingSet);
            }

            sign = (sign != n2.sign);
        }

        /// <summary>
        /// Squares the number.
        /// </summary>
        public void Square()
        {
            int Length = digitArray.Length;

            //Inner loop zero-mul avoidance
            int maxDigit = 0;
            for (int i = Length - 1; i >= 0; i--)
            {
                if (digitArray[i] != 0)
                {
                    maxDigit = i + 1;
                    break;
                }
            }

            //Result is zero, 'this' is unchanged
            if (maxDigit == 0) return;

            //Temp storage for source (both working sets are used by the calculation)
            uint[] thisTemp = new uint[Length];
            for (int i = 0; i < Length; i++)
            {
                thisTemp[i] = digitArray[i];
                digitArray[i] = 0;
            }

            UInt32 [] workingSet2 = new UInt32[Length];

            for (int i = 0; i < Length; i++)
            {
                //Clear the working set
                for (int j = 0; j < i; j++)
                {
                    workingSet[j] = 0;
                    workingSet2[j] = 0;
                }

                workingSet2[i] = 0;

                ulong digit = thisTemp[i];
                if (digit == 0) continue;

                for (int j = 0; j + i < Length && j < maxDigit; j++)
                {
                    //Multiply n1 by each of the integer digits of n2.
                    ulong temp = (ulong)thisTemp[j] * digit;
                    //n1.workingSet stores the low bits of each piecewise multiplication
                    workingSet[j + i] = (uint)(temp & 0xffffffff);
                    if (j + i + 1 < Length)
                    {
                        //n2.workingSet stores the high bits of each multiplication
                        workingSet2[j + i + 1] = (uint)(temp >> 32);
                    }
                }

                AddInternalBits(workingSet);
                AddInternalBits(workingSet2);
            }

            sign = false;
        }

        /// <summary>
        /// Used for floating-point multiplication
        /// Stores the high bits of the multiplication only (the carry bit from the
        /// lower bits is missing, so the true answer might be 1 greater).
        /// </summary>
        /// <param name="n2"></param>
        public void MulHi(BigInt n2)
        {
            if (n2.digitArray.Length != digitArray.Length) MakeSafe(ref n2);

            int Length = n2.digitArray.Length;

            //Inner loop zero-mul avoidance
            int maxDigit = 0;
            for (int i = Length - 1; i >= 0; i--)
            {
                if (digitArray[i] != 0)
                {
                    maxDigit = i + 1;
                    break;
                }
            }

            //Result is zero, 'this' is unchanged
            if (maxDigit == 0) return;

            //Temp storage for source (both working sets are used by the calculation)
            uint[] thisTemp = new uint[Length];
            for (int i = 0; i < Length; i++)
            {
                thisTemp[i] = digitArray[i];
                digitArray[i] = 0;
            }

            for (int i = 0; i < Length; i++)
            {
                //Clear the working set
                for (int j = 0; j < Length; j++)
                {
                    workingSet[j] = 0;
                    n2.workingSet[j] = 0;
                }

                n2.workingSet[i] = 0;

                ulong digit = n2.digitArray[i];
                if (digit == 0) continue;

                //Only the high bits
                if (maxDigit + i < Length - 1) continue;

                for (int j = 0; j < maxDigit; j++)
                {
                    if (j + i + 1 < Length) continue;
                    //Multiply n1 by each of the integer digits of n2.
                    ulong temp = (ulong)thisTemp[j] * digit;
                    //n1.workingSet stores the low bits of each piecewise multiplication
                    if (j + i >= Length)
                    {
                        workingSet[j + i - Length] = (uint)(temp & 0xffffffff);
                    }
                    
                    //n2.workingSet stores the high bits of each multiplication
                    n2.workingSet[j + i + 1 - Length] = (uint)(temp >> 32);
                }

                AddInternalBits(workingSet);
                AddInternalBits(n2.workingSet);
            }

            sign = (sign != n2.sign);
        }

        /// <summary>
        /// Floating-point helper function.
        /// Squares the number and keeps the high bits of the calculation.
        /// Takes a temporary BigInt as a working set.
        /// </summary>
        public void SquareHiFast(BigInt scratch)
        {
            int Length = digitArray.Length;
            uint[] tempDigits = scratch.digitArray;
            uint[] workingSet2 = scratch.workingSet;

            //Temp storage for source (both working sets are used by the calculation)
            for (int i = 0; i < Length; i++)
            {
                tempDigits[i] = digitArray[i];
                digitArray[i] = 0;
            }

            for (int i = 0; i < Length; i++)
            {
                //Clear the working set
                for (int j = i; j < Length; j++)
                {
                    workingSet[j] = 0;
                    workingSet2[j] = 0;
                }

                if (i - 1 >= 0) workingSet[i - 1] = 0;

                ulong digit = tempDigits[i];
                if (digit == 0) continue;

                for (int j = 0; j < Length; j++)
                {
                    if (j + i + 1 < Length) continue;
                    //Multiply n1 by each of the integer digits of n2.
                    ulong temp = (ulong)tempDigits[j] * digit;
                    //n1.workingSet stores the low bits of each piecewise multiplication
                    if (j + i >= Length)
                    {
                        workingSet[j + i - Length] = (uint)(temp & 0xffffffff);
                    }

                    //n2.workingSet stores the high bits of each multiplication
                    workingSet2[j + i + 1 - Length] = (uint)(temp >> 32);
                }

                AddInternalBits(workingSet);
                AddInternalBits(workingSet2);
            }

            sign = false;
        }

        /// <summary>
        /// This uses the schoolbook division algorithm, as decribed by http://www.treskal.com/kalle/exjobb/original-report.pdf
        /// Algorithms 3.1 (implemented by Div_31) and 3.2 (implemented by Div_32)
        /// </summary>
        /// <param name="n2"></param>
        public void Div(BigInt n2)
        {
            if (n2.digitArray.Length != digitArray.Length) MakeSafe(ref n2);

            int OldLength = digitArray.Length;

            //First, we need to prepare the operands for division using Div_32, which requires
            //That the most significant digit of n2 be set. To do this, we need to shift n2 (and therefore n1) up.
            //This operation can potentially increase the precision of the operands.
            int shift = MakeSafeDiv(this, n2);

            BigInt Q = new BigInt(this, this.pres, true);
            BigInt R = new BigInt(this, this.pres, true);

            Div_32(this, n2, Q, R);

            //Restore n2 to its pre-shift value
            n2.RSH(shift);
            AssignInt(Q);
            sign = (sign != n2.sign);

            //Reset the lengths of the operands
            SetNumDigits(OldLength);
            n2.SetNumDigits(OldLength);
        }

        /// <summary>
        /// This function is used for floating-point division.
        /// </summary>
        /// <param name="n2"></param>
        //Given two numbers:
        //  In floating point 1 <= a, b < 2, meaning that both numbers have their top bits set.
        //  To calculate a / b, maintaining precision, we:
        //    1. Double the number of digits available to both numbers.
        //    2. set a = a * 2^d (where d is the number of digits)
        //    3. calculate the quotient a <- q:  2^(d-1) <= q < 2^(d+1)
        //    4. if a >= 2^d, s = 1, else s = 0
        //    6. shift a down by s, and undo the precision extension
        //    7. return 1 - shift (change necessary to exponent)
        public int DivAndShift(BigInt n2)
        {
            if (n2.IsZero()) return -1;
            if (digitArray.Length != n2.digitArray.Length) MakeSafe(ref n2);

            int oldLength = digitArray.Length;

            //Double the number of digits, and shift a into the higher digits.
            Pad();
            n2.Extend();

            //Do the divide (at double precision, ouch!)
            Div(n2);

            //Shift down if 'this' >= 2^d
            int ret = 1;

            if (digitArray[oldLength] != 0)
            {
                RSH(1);
                ret--;
            }

            SetNumDigits(oldLength);
            n2.SetNumDigits(oldLength);

            return ret;
        }

        /// <summary>
        /// Calculates 'this' mod n2 (using the schoolbook division algorithm as above)
        /// </summary>
        /// <param name="n2"></param>
        public void Mod(BigInt n2)
        {
            if (n2.digitArray.Length != digitArray.Length) MakeSafe(ref n2);

            int OldLength = digitArray.Length;

            //First, we need to prepare the operands for division using Div_32, which requires
            //That the most significant digit of n2 be set. To do this, we need to shift n2 (and therefore n1) up.
            //This operation can potentially increase the precision of the operands.
            int shift = MakeSafeDiv(this, n2);

            BigInt Q = new BigInt(this.pres);
            BigInt R = new BigInt(this.pres);

            Q.digitArray = new UInt32[this.digitArray.Length];
            R.digitArray = new UInt32[this.digitArray.Length];

            Div_32(this, n2, Q, R);

            //Restore n2 to its pre-shift value
            n2.RSH(shift);
            R.RSH(shift);
            R.sign = (sign != n2.sign);
            AssignInt(R);

            //Reset the lengths of the operands
            SetNumDigits(OldLength);
            n2.SetNumDigits(OldLength);
        }

        /// <summary>
        /// Logical left shift
        /// </summary>
        /// <param name="shift"></param>
        public void LSH(int shift)
        {
            if (shift <= 0) return;
            int length = digitArray.Length;
            int digits = shift >> 5;
            int rem = shift & 31;

            for (int i = length - 1; i >= digits; i--)
            {
                digitArray[i] = digitArray[i - digits];
            }

            if (digits > 0)
            {
                for (int i = digits - 1; i >= 0; i--)
                {
                    digitArray[i] = 0;
                }
            }

            UInt64 lastShift = 0;

            for (int i = 0; i < length; i++)
            {
                UInt64 temp = (((UInt64)digitArray[i]) << rem) | lastShift;
                digitArray[i] = (UInt32)(temp & 0xffffffff);
                lastShift = temp >> 32;
            }
        }

        /// <summary>
        /// Logical right-shift
        /// </summary>
        /// <param name="shift"></param>
        public void RSH(int shift)
        {
            if (shift < 0) return;
            int length = digitArray.Length;
            int digits = shift >> 5;
            int rem = shift & 31;

            for (int i = 0; i < length - digits; i++)
            {
                digitArray[i] = digitArray[i + digits];
            }

            int start = (length - digits);
            if (start < 0) start = 0;

            for (int i = start; i < length; i++)
            {
                digitArray[i] = 0;
            }

            UInt64 lastShift = 0;

            for (int i = length - 1; i >= 0; i--)
            {
                UInt64 temp = ((((UInt64)digitArray[i]) << 32) >> rem) | lastShift;
                digitArray[i] = (UInt32)(temp >> 32);
                lastShift = (temp & 0xffffffff) << 32;
            }
        }

        /// <summary>
        /// Changes the sign of the number
        /// </summary>
        public void Negate()
        {
            sign = !sign;
        }

        /// <summary>
        /// Increments the number.
        /// </summary>
        public void Increment()
        {
            if (sign)
            {
                DecrementInt();
            }
            else
            {
                IncrementInt();
            }
        }

        /// <summary>
        /// Decrements the number.
        /// </summary>
        public void Decrement()
        {
            if (sign)
            {
                IncrementInt();
            }
            else
            {
                DecrementInt();
            }
        }

        /// <summary>
        /// Calculates the factorial 'this'!
        /// </summary>
        public void Factorial()
        {
            if (sign) return;

            //Clamp to a reasonable range.
            int factToUse = (int)(digitArray[0]);
            if (factToUse > 65536) factToUse = 65536;

            Zero();
            digitArray[0] = 1;

            for (uint i = 1; i <= factToUse; i++)
            {
                MulInternal(i);
            }
        }

        /// <summary>
        /// Calculates 'this'^power
        /// </summary>
        /// <param name="power"></param>
        public void Power(BigInt power)
        {
            if (power.IsZero() || power.sign)
            {
                Zero();
                digitArray[0] = 1;
                return;
            }

            BigInt pow = new BigInt(power);
            BigInt temp = new BigInt(this);
            BigInt powTerm = new BigInt(this);

            pow.Decrement();
            for (; !pow.IsZero(); pow.RSH(1))
            {
                if ((pow.digitArray[0] & 1) == 1)
                {
                    temp.Mul(powTerm);
                }

                powTerm.Square();
            }

            Assign(temp);
        }

        //***************** Comparison member functions *****************

        /// <summary>
        /// returns true if this bigint == 0
        /// </summary>
        /// <returns></returns>
        public bool IsZero()
        {
            for (int i = 0; i < digitArray.Length; i++)
            {
                if (digitArray[i] != 0) return false;
            }

            return true;
        }

        /// <summary>
        /// true iff n2 (precision adjusted to this) is less than 'this'
        /// </summary>
        /// <param name="n2"></param>
        /// <returns></returns>
        public bool LessThan(BigInt n2)
        {
            if (digitArray.Length != n2.digitArray.Length) MakeSafe(ref n2);

            if (sign)
            {
                if (!n2.sign) return true;
                return GtInt(this, n2);
            }
            else
            {
                if (n2.sign) return false;
                return LtInt(this, n2);
            }
        }

        /// <summary>
        /// true iff n2 (precision adjusted to this) is greater than 'this'
        /// </summary>
        /// <param name="n2"></param>
        /// <returns></returns>
        public bool GreaterThan(BigInt n2)
        {
            if (digitArray.Length != n2.digitArray.Length) MakeSafe(ref n2);

            if (sign)
            {
                if (!n2.sign) return false;
                return LtInt(this, n2);
            }
            else
            {
                if (n2.sign) return true;
                return GtInt(this, n2);
            }
        }

        /// <summary>
        /// Override of base-class equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            BigInt n2 = ((BigInt)obj);
            return Equals(n2);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Int32)digitArray[0];
        }

        /// <summary>
        /// True iff n2 (precision-adjusted to this) == n1
        /// </summary>
        /// <param name="n2"></param>
        /// <returns></returns>
        public bool Equals(BigInt n2)
        {
            if (IsZero() && n2.IsZero()) return true;

            if (sign != n2.sign) return false;

            int Length = digitArray.Length;
            if (n2.digitArray.Length != Length) MakeSafe(ref n2);

            for (int i = 0; i < Length; i++)
            {
                if (digitArray[i] != n2.digitArray[i]) return false;
            }

            return true;
        }

        //******************* Bitwise member functions ********************

        /// <summary>
        /// Takes the bitwise complement of the number
        /// </summary>
        public void Complement()
        {
            int Length = digitArray.Length;

            for (int i = 0; i < Length; i++)
            {
                digitArray[i] = ~digitArray[i];
            }
        }

        /// <summary>
        /// Bitwise And
        /// </summary>
        /// <param name="n2"></param>
        public void And(BigInt n2)
        {
            int Length = digitArray.Length;
            if (n2.digitArray.Length != Length) MakeSafe(ref n2);

            for (int i = 0; i < Length; i++)
            {
                digitArray[i] &= n2.digitArray[i];
            }

            sign &= n2.sign;
        }

        /// <summary>
        /// Bitwise Or
        /// </summary>
        /// <param name="n2"></param>
        public void Or(BigInt n2)
        {
            int Length = digitArray.Length;
            if (n2.digitArray.Length != Length) MakeSafe(ref n2);

            for (int i = 0; i < Length; i++)
            {
                digitArray[i] |= n2.digitArray[i];
            }

            sign |= n2.sign;
        }

        /// <summary>
        /// Bitwise Xor
        /// </summary>
        /// <param name="n2"></param>
        public void Xor(BigInt n2)
        {
            int Length = digitArray.Length;
            if (n2.digitArray.Length != Length) MakeSafe(ref n2);

            for (int i = 0; i < Length; i++)
            {
                digitArray[i] ^= n2.digitArray[i];
            }

            sign ^= n2.sign;
        }

        //*************** Fast Static Arithmetic Functions *****************

        /// <summary>
        /// Adds n1 and n2 and puts result in dest, without intermediate memory allocation
        /// (unsafe if n1 and n2 disagree in precision, safe even if dest is n1 or n2)
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        public static void AddFast(BigInt dest, BigInt n1, BigInt n2)
        {
            //We cast to the highest input precision...
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);

            //Then we up the output precision if less than the input precision.
            if (dest.digitArray.Length < n1.digitArray.Length) n1.MakeSafe(ref dest);

            int Length = n1.digitArray.Length;

            if (n1.sign == n2.sign)
            {
                //Copies sources into digit array and working set for all cases, to avoid
                //problems when dest is n1 or n2
                for (int i = 0; i < Length; i++)
                {
                    dest.workingSet[i] = n2.digitArray[i];
                    dest.digitArray[i] = n1.digitArray[i];
                }
                dest.AddInternalBits(dest.workingSet);
                dest.sign = n1.sign;
            }
            else
            {
                bool lessThan = LtInt(n1, n2);

                if (lessThan)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        dest.workingSet[i] = n1.digitArray[i];
                        dest.digitArray[i] = n2.digitArray[i];
                    }
                    dest.SubInternalBits(dest.workingSet);
                    dest.sign = !n1.sign;
                }
                else
                {
                    for (int i = 0; i < Length; i++)
                    {
                        dest.workingSet[i] = n2.digitArray[i];
                        dest.digitArray[i] = n1.digitArray[i];
                    }
                    dest.SubInternalBits(dest.workingSet);
                    dest.sign = n1.sign;
                }
            }
        }

        /// <summary>
        /// Adds n1 and n2 and puts result in dest, without intermediate memory allocation
        /// (unsafe if n1 and n2 disagree in precision, safe even if dest is n1 or n2)
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        public static void SubFast(BigInt dest, BigInt n1, BigInt n2)
        {
            //We cast to the highest input precision...
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);

            //Then we up the output precision if less than the input precision.
            if (dest.digitArray.Length < n1.digitArray.Length) n1.MakeSafe(ref dest);

            int Length = n1.digitArray.Length;

            if (n1.sign != n2.sign)
            {
                //Copies sources into digit array and working set for all cases, to avoid
                //problems when dest is n1 or n2
                for (int i = 0; i < Length; i++)
                {
                    dest.workingSet[i] = n2.digitArray[i];
                    dest.digitArray[i] = n1.digitArray[i];
                }
                dest.AddInternalBits(dest.workingSet);
                dest.sign = n1.sign;
            }
            else
            {
                bool lessThan = LtInt(n1, n2);

                if (lessThan)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        dest.workingSet[i] = n1.digitArray[i];
                        dest.digitArray[i] = n2.digitArray[i];
                    }
                    dest.SubInternalBits(dest.workingSet);
                    dest.sign = !n1.sign;
                }
                else
                {
                    for (int i = 0; i < Length; i++)
                    {
                        dest.workingSet[i] = n2.digitArray[i];
                        dest.digitArray[i] = n1.digitArray[i];
                    }
                    dest.SubInternalBits(dest.workingSet);
                    dest.sign = n1.sign;
                }
            }
        }

        //*************** Static Arithmetic Functions ***************

        /// <summary>
        /// signed addition of 2 numbers.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Add(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt result; 

            if (n1.sign == n2.sign)
            {
                result = new BigInt(n1);
                result.AddInternal(n2);
                result.sign = n1.sign;
            }
            else
            {
                bool lessThan = LtInt(n1, n2);

                if (lessThan)
                {
                    result = new BigInt(n2);
                    result.SubInternal(n1);
                    result.sign = !n1.sign;
                }
                else
                {
                    result = new BigInt(n1);
                    result.SubInternal(n2);
                    result.sign = n1.sign;
                }
            }

            return result;
        }

        /// <summary>
        /// signed subtraction of 2 numbers.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Sub(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt result;

            if ((n1.sign && !n2.sign) || (!n1.sign && n2.sign))
            {
                result = new BigInt(n1);
                result.AddInternal(n2);
                result.sign = n1.sign;
            }
            else
            {
                bool lessThan = LtInt(n1, n2);

                if (lessThan)
                {
                    result = new BigInt(n2);
                    result.SubInternal(n1);
                    result.sign = !n1.sign;
                }
                else
                {
                    result = new BigInt(n1);
                    result.SubInternal(n2);
                    result.sign = n1.sign;
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplication of two BigInts
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Mul(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            
            BigInt result = new BigInt(n1);
            result.Mul(n2);
            return result;
        }

        /// <summary>
        /// True arbitrary precision divide.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Div(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt res = new BigInt(n1);
            res.Div(n2);
            return res;
        }

        /// <summary>
        /// True arbitrary-precision mod operation
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Mod(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt res = new BigInt(n1);
            res.Mod(n2);
            return res;
        }

        /// <summary>
        /// Unsigned multiplication of a BigInt by a small number
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Mul(BigInt n1, uint n2)
        {
            BigInt result = new BigInt(n1);
            result.MulInternal(n2);
            return result;
        }

        /// <summary>
        /// Division of a BigInt by a small (unsigned) number
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Div(BigInt n1, uint n2)
        {
            BigInt result = new BigInt(n1);
            result.DivInternal(n2);
            return result;
        }

        /// <summary>
        /// Division and remainder of a BigInt by a small (unsigned) number
        /// n1 / n2 = div remainder mod
        /// </summary>
        /// <param name="n1">The number to divide (dividend)</param>
        /// <param name="n2">The number to divide by (divisor)</param>
        /// <param name="div">The quotient (output parameter)</param>
        /// <param name="mod">The remainder (output parameter)</param>
        public static void DivMod(BigInt n1, uint n2, out BigInt div, out BigInt mod)
        {
            div = Div(n1, n2);
            mod = Mul(div, n2);
            mod = Sub(n1, mod);
        }

        //**************** Static Comparison Functions ***************

        /// <summary>
        /// true iff n1 is less than n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static bool LessThan(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);

            if (n1.sign)
            {
                if (!n2.sign) return true;
                return GtInt(n1, n2);
            }
            else
            {
                if (n2.sign) return false;
                return LtInt(n1, n2);
            }
        }

        /// <summary>
        /// true iff n1 is greater than n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static bool GreaterThan(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);

            if (n1.sign)
            {
                if (!n2.sign) return false;
                return LtInt(n1, n2);
            }
            else
            {
                if (n2.sign) return true;
                return GtInt(n1, n2);
            }
        }

        /// <summary>
        /// true iff n1 == n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static bool Equals(BigInt n1, BigInt n2)
        {
            return n1.Equals(n2);
        }

        //***************** Static Bitwise Functions *****************

        /// <summary>
        /// Bitwise And
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt And(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt res = new BigInt(n1);
            res.And(n2);
            return res;
        }

        /// <summary>
        /// Bitwise Or
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Or(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt res = new BigInt(n1);
            res.Or(n2);
            return res;
        }

        /// <summary>
        /// Bitwise Xor
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static BigInt Xor(BigInt n1, BigInt n2)
        {
            if (n1.digitArray.Length != n2.digitArray.Length) MakeSafe(ref n1, ref n2);
            BigInt res = new BigInt(n1);
            res.And(n2);
            return res;
        }

        //**************** Static Operator Overloads *****************

        /// <summary>
        /// Addition operator
        /// </summary>
        public static BigInt operator +(BigInt n1, BigInt n2)
        {
            return Add(n1, n2);
        }

        /// <summary>
        /// The subtraction operator
        /// </summary>
        public static BigInt operator -(BigInt n1, BigInt n2)
        {
            return Sub(n1, n2);
        }

        /// <summary>
        /// The multiplication operator
        /// </summary>
        public static BigInt operator *(BigInt n1, BigInt n2)
        {
            return Mul(n1, n2);
        }

        /// <summary>
        /// The division operator
        /// </summary>
        public static BigInt operator /(BigInt n1, BigInt n2)
        {
            return Div(n1, n2);
        }

        /// <summary>
        /// The remainder (mod) operator
        /// </summary>
        public static BigInt operator %(BigInt n1, BigInt n2)
        {
            return Mod(n1, n2);
        }

        /// <summary>
        /// The left-shift operator
        /// </summary>
        public static BigInt operator <<(BigInt n1, int n2)
        {
            BigInt res = new BigInt(n1);
            res.LSH(n2);
            return res;
        }

        /// <summary>
        /// The right-shift operator
        /// </summary>
        public static BigInt operator >>(BigInt n1, int n2)
        {
            BigInt res = new BigInt(n1);
            res.RSH(n2);
            return res;
        }

        /// <summary>
        /// The less than operator
        /// </summary>
        public static bool operator <(BigInt n1, BigInt n2)
        {
            return LessThan(n1, n2);
        }

        /// <summary>
        /// The greater than operator
        /// </summary>
        public static bool operator >(BigInt n1, BigInt n2)
        {
            return GreaterThan(n1, n2);
        }

        /// <summary>
        /// The less than or equal to operator
        /// </summary>
        public static bool operator <=(BigInt n1, BigInt n2)
        {
            return !GreaterThan(n1, n2);
        }

        /// <summary>
        /// The greater than or equal to operator
        /// </summary>
        public static bool operator >=(BigInt n1, BigInt n2)
        {
            return !LessThan(n1, n2);
        }

        /// <summary>
        /// The equality operator
        /// </summary>
        public static bool operator ==(BigInt n1, BigInt n2)
        {
            return Equals(n1, n2);
        }

        /// <summary>
        /// The inequality operator
        /// </summary>
        public static bool operator !=(BigInt n1, BigInt n2)
        {
            return !Equals(n1, n2);
        }

        /// <summary>
        /// The bitwise AND operator
        /// </summary>
        public static BigInt operator &(BigInt n1, BigInt n2)
        {
            return And(n1, n2);
        }

        /// <summary>
        /// The bitwise OR operator
        /// </summary>
        public static BigInt operator |(BigInt n1, BigInt n2)
        {
            return Or(n1, n2);
        }

        /// <summary>
        /// The bitwise eXclusive OR operator
        /// </summary>
        public static BigInt operator ^(BigInt n1, BigInt n2)
        {
            return Xor(n1, n2);
        }

        /// <summary>
        /// The increment operator
        /// </summary>
        public static BigInt operator ++(BigInt n1)
        {
            n1.Increment();
            return n1;
        }

        /// <summary>
        /// The decrement operator
        /// </summary>
        public static BigInt operator --(BigInt n1)
        {
            n1.Decrement();
            return n1;
        }

        //**************** Private Member Functions *****************

        /// <summary>
        /// Unsigned multiplication and assignment by a small number
        /// </summary>
        /// <param name="n2"></param>
        private void MulInternal(uint n2)
        {
            int Length = digitArray.Length;
            ulong n2long = (ulong)n2;

            for (int i = 0; i < Length; i++)
            {
                workingSet[i] = 0;
            }

            for (int i = 0; i < Length; i++)
            {
                if (digitArray[i] == 0) continue;
                ulong temp = (ulong)digitArray[i] * n2long;
                digitArray[i] = (uint)(temp & 0xffffffff);
                if (i + 1 < Length) workingSet[i + 1] = (uint)(temp >> 32);
            }

            AddInternalBits(workingSet);
        }

        /// <summary>
        /// Unsigned division and assignment by a small number
        /// </summary>
        /// <param name="n2"></param>
        private void DivInternal(uint n2)
        {
            int Length = digitArray.Length;
            ulong carry = 0;

            //Divide each digit by the small number.
            for (int i = Length - 1; i >= 0; i--)
            {
                ulong temp = (ulong)digitArray[i] + (carry << 32);
                digitArray[i] = (uint)(temp / (ulong)n2);
                carry = temp % (ulong)n2;
            }
        }

        /// <summary>
        /// Adds a signed integer to the number.
        /// </summary>
        /// <param name="n1"></param>
        private void AddInternal(int n1)
        {
            if (n1 < 0)
            {
                SubInternal(-n1);
                return;
            }

            uint carry = 0;
            int length = digitArray.Length;

            for (int i = 0; i < length && !(n1 == 0 && carry == 0); i++)
            {
                uint temp = digitArray[i];
                digitArray[i] += (uint)n1 + carry;

                carry = (digitArray[i] <= temp) ? 1u: 0u;
                
                n1 = 0;
            }
        }

        /// <summary>
        /// Subtract a signed integer from the number.
        /// This is internal because it will fail spectacularly if this number is negative or if n1 is bigger than this number.
        /// </summary>
        /// <param name="n1"></param>
        private void SubInternal(int n1)
        {
            if (n1 < 0)
            {
                AddInternal(-n1);
                return;
            }

            uint carry = 0;
            int length = digitArray.Length;

            for (int i = 0; i < length && !(n1 == 0 && carry == 0); i++)
            {
                uint temp = digitArray[i];
                digitArray[i] -= ((uint)n1 + carry);

                carry = (digitArray[i] >= temp) ? 1u: 0u;

                n1 = 0;
            }
        }

        /// <summary>
        /// Adds a signed integer to the number.
        /// </summary>
        /// <param name="n1"></param>
        private bool AddInternal(uint n1)
        {
            uint carry = 0;
            int length = digitArray.Length;

            for (int i = 0; i < length && !(n1 == 0 && carry == 0); i++)
            {
                uint temp = digitArray[i];
                digitArray[i] += n1 + carry;

                carry = (digitArray[i] <= temp) ? 1u: 0u;

                n1 = 0;
            }

            return (carry != 0);
        }

        /// <summary>
        /// Internally subtracts a uint from the number (sign insensitive)
        /// </summary>
        /// <param name="n1"></param>
        /// <returns></returns>
        private bool SubInternal(uint n1)
        {
            uint carry = 0;
            int length = digitArray.Length;

            for (int i = 0; i < length && !(n1 == 0 && carry == 0); i++)
            {
                uint temp = digitArray[i];
                digitArray[i] -= (n1 + carry);

                carry = (digitArray[i] >= temp) ? 1u: 0u;

                n1 = 0;
            }

            return (carry != 0);
        }

        /// <summary>
        /// Internal increment function (sign insensitive)
        /// </summary>
        private bool IncrementInt()
        {
            uint carry = 1;

            int length = digitArray.Length;

            for (int i = 0; i < length && carry != 0; i++)
            {
                uint temp = digitArray[i];
                digitArray[i]++;

                if (digitArray[i] > temp) carry = 0;
            }

            return (carry != 0);
        }

        /// <summary>
        /// Internal increment function (sign insensitive)
        /// </summary>
        private bool DecrementInt()
        {
            uint carry = 1;

            int length = digitArray.Length;

            for (int i = 0; i < length && carry != 0; i++)
            {
                uint temp = digitArray[i];
                digitArray[i]--;

                if (digitArray[i] < temp) carry = 0;
            }

            return (carry != 0);
        }

        /// <summary>
        /// Used to add a digit array to a big int.
        /// </summary>
        /// <param name="digitsToAdd"></param>
        private uint AddInternalBits(uint[] digitsToAdd)
        {
            uint carry = 0;

            int Length = digitArray.Length;

            for (int i = 0; i < Length; i++)
            {
                //Necessary because otherwise the carry calculation could go bad.
                if (digitsToAdd[i] == 0 && carry == 0) continue;

                uint temp = digitArray[i];
                digitArray[i] += (digitsToAdd[i] + carry);

                carry = 0;
                if (digitArray[i] <= temp) carry = 1;
            }

            return carry;
        }

        /// <summary>
        /// Used to add with matching signs (true addition of the digit arrays)
        /// This is internal because it will fail spectacularly if n1 is negative.
        /// </summary>
        /// <param name="n1"></param>
        private uint AddInternal(BigInt n1)
        {
            return AddInternalBits(n1.digitArray);
        }

        private uint SubInternalBits(uint[] digitsToAdd)
        {
            uint carry = 0;

            int Length = digitArray.Length;

            for (int i = 0; i < Length; i++)
            {
                //Necessary because otherwise the carry calculation could go bad.
                if (digitsToAdd[i] == 0 && carry == 0) continue;

                uint temp = digitArray[i];
                digitArray[i] -= (digitsToAdd[i] + carry);

                carry = 0;
                if (digitArray[i] >= temp) carry = 1;
            }

            return carry;
        }

        /// <summary>
        /// Used to subtract n1 (true subtraction of digit arrays) - n1 must be less than or equal to this number
        /// </summary>
        /// <param name="n1"></param>
        private uint SubInternal(BigInt n1)
        {
            return SubInternalBits(n1.digitArray);
        }

        /// <summary>
        /// Returns the length of the BigInt in 32-bit words for a given decimal precision
        /// </summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static int GetRequiredDigitsForPrecision(PrecisionSpec precision)
        {
            int bits = precision.NumBits;
            return ((bits - 1) >> 5) + 1;
        }

        /// <summary>
        /// Initialises the BigInt to a desired decimal precision
        /// </summary>
        /// <param name="precision"></param>
        private void Init(PrecisionSpec precision)
        {
            int numDigits = GetRequiredDigitsForPrecision(precision);
            digitArray = new uint[numDigits];
            workingSet = new uint[numDigits];
            pres = precision;
        }

        /// <summary>
        /// Initialises the BigInt from a string, given a base and precision
        /// </summary>
        /// <param name="init"></param>
        /// <param name="precision"></param>
        /// <param name="numberBase"></param>
        private void InitFromString(string init, PrecisionSpec precision, int numberBase)
        {
            PrecisionSpec test;
            if (numberBase == 2)
            {
                test = new PrecisionSpec(init.Length, PrecisionSpec.BaseType.BIN);
            }
            else if (numberBase == 8)
            {
                test = new PrecisionSpec(init.Length, PrecisionSpec.BaseType.OCT);
            }
            else if (numberBase == 10)
            {
                test = new PrecisionSpec(init.Length, PrecisionSpec.BaseType.DEC);
            }
            else if (numberBase == 16)
            {
                test = new PrecisionSpec(init.Length, PrecisionSpec.BaseType.HEX);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            //if (test.NumBits > precision.NumBits) precision = test;
            Init(precision);
            FromStringInt(init, numberBase);
        }

        //************ Helper Functions for floating point *************

        /// <summary>
        /// Returns true if only the top bit is set: i.e. if the floating-point number is a power of 2
        /// </summary>
        /// <returns></returns>
        public bool IsTopBitOnlyBit()
        {
            int length = digitArray.Length;

            if (digitArray[length - 1] != 0x80000000u) return false;
            length--;
            for (int i = 0; i < length; i++)
            {
                if (digitArray[i] != 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Zeroes the n most significant bits of the number
        /// </summary>
        /// <param name="bits"></param>
        public void ZeroBitsHigh(int bits)
        {
            //Already done.
            if (bits <= 0) return;

            int length = digitArray.Length;

            //The entire digit array.
            if ((bits >> 5) > length)
            {
                bits = length << 5;
            }

            int remBits = (bits & 31);
            int startDigit = length - ((bits >> 5) + 1);

            if (remBits != 0)
            {
                digitArray[startDigit] = digitArray[startDigit] & (0xffffffffu >> remBits);
            }

            for (int i = startDigit + 1; i < length; i++)
            {
                digitArray[i] = 0;
            }
        }

        /// <summary>
        /// Zeroes the least-significant n bits.
        /// </summary>
        /// <param name="bits"></param>
        public void ZeroBits(int bits)
        {
            //Already done.
            if (bits <= 0) return;

            //The entire digit array.
            if ((bits >> 5) > digitArray.Length)
            {
                bits = digitArray.Length << 5;
            }

            int remBits = (bits & 31);
            int startDigit = bits >> 5;
            
            if (remBits != 0)
            {
                UInt32 startMask = 0xffffffffu & ~(UInt32)(((1 << remBits) - 1));
                digitArray[startDigit] = digitArray[startDigit] & startMask;
            }

            for (int i = startDigit - 1; i >= 0; i--)
            {
                digitArray[i] = 0;
            }
        }

        /// <summary>
        /// Sets the number to 0
        /// </summary>
        public void Zero()
        {
            int length = digitArray.Length;

            for (int i = 0; i < length; i++)
            {
                digitArray[i] = 0;
            }
        }

        /// <summary>
        /// Rounds off the least significant bits of the number.
        /// Can only round off up to 31 bits.
        /// </summary>
        /// <param name="bits">number of bits to round</param>
        /// <returns></returns>
        public bool Round(int bits)
        {
            //Always less than 32 bits, please!
            if (bits < 32)
            {
                uint pow2 = 1u << bits;
                uint test = digitArray[0] & (pow2 >> 1);

                //Zero the lower bits
                digitArray[0] = digitArray[0] & ~(pow2 - 1);

                if (test != 0)
                {
                    bool bRet = AddInternal(pow2);
                    digitArray[digitArray.Length - 1] = digitArray[digitArray.Length - 1] | 0x80000000;
                    return bRet;
                }
            }

            return false;
        }

        /// <summary>
        /// Used for casting between BigFloats of different precisions - this assumes
        /// that the number is a normalised mantissa.
        /// </summary>
        /// <param name="n2"></param>
        /// <returns>true if a round-up caused the high bits to become zero</returns>
        public bool AssignHigh(BigInt n2)
        {
            int length = digitArray.Length;
            int length2 = n2.digitArray.Length;
            int minWords = (length < length2) ? length: length2;
            bool bRet = false;

            for (int i = 1; i <= minWords; i++)
            {
                digitArray[length - i] = n2.digitArray[length2 - i];
            }

            if (length2 > length && n2.digitArray[length2 - (length + 1)] >= 0x80000000)
            {
                bRet = IncrementInt();

                //Because we are assuming normalisation, we set the top bit (it will already be set if
                //bRet is false.
                digitArray[length - 1] = digitArray[length - 1] | 0x80000000;
            }

            sign = n2.sign;

            return bRet;
        }

        /// <summary>
        /// Used for casting between long ints or doubles and floating-point numbers
        /// </summary>
        /// <param name="digits"></param>
        public void SetHighDigits(Int64 digits)
        {
            digitArray[digitArray.Length - 1] = (uint)(digits >> 32);
            if (digitArray.Length > 1) digitArray[digitArray.Length - 2] = (uint)(digits & 0xffffffff);
        }

        /// <summary>
        /// Used for casting between ints and doubles or floats.
        /// </summary>
        /// <param name="digit"></param>
        public void SetHighDigit(UInt32 digit)
        {
            digitArray[digitArray.Length - 1] = digit;
        }

        /// <summary>
        /// Helper function for floating-point - extends the number to twice the precision
        /// and shifts the digits into the upper bits.
        /// </summary>
        public void Pad()
        {
            int length = digitArray.Length;
            int digits = length << 1;

            UInt32[] newDigitArray = new UInt32[digits];
            workingSet = new UInt32[digits];

            for (int i = 0; i < length; i++)
            {
                newDigitArray[i + length] = digitArray[i];
            }

            digitArray = newDigitArray;
        }

        /// <summary>
        /// Helper function for floating-point - extends the number to twice the precision...
        /// This is a necessary step in floating-point division.
        /// </summary>
        public void Extend()
        {
            SetNumDigits(digitArray.Length * 2);
        }

        /// <summary>
        /// Gets the highest big of the integer (used for floating point stuff)
        /// </summary>
        /// <returns></returns>
        public uint GetTopBit()
        {
            return (digitArray[digitArray.Length - 1] >> 31);
        }

        /// <summary>
        /// Used for floating point multiplication, this shifts the number so that
        /// the highest bit is set, and returns the number of places shifted.
        /// </summary>
        /// <returns></returns>
        public int Normalise()
        {
            if (IsZero()) return 0;

            int MSD = GetMSD();
            int digitShift = (digitArray.Length - (MSD + 1));
            int bitShift = (31 - GetMSB(digitArray[MSD])) + (digitShift << 5);
            LSH(bitShift);
            return bitShift;
        }

        /// <summary>
        /// Gets the most significant bit
        /// </summary>
        /// <param name="value">the input to search for the MSB in</param>
        /// <returns>-1 if the input was zero, the position of the MSB otherwise</returns>
        public static int GetMSB(UInt32 value)
        {
            if (value == 0) return -1;

            uint mask1 = 0xffff0000;
            uint mask2 = 0xff00;
            uint mask3 = 0xf0;
            uint mask4 = 0xc;   //1100 in binary
            uint mask5 = 0x2;   //10 in binary

            int iPos = 0;

            //Unrolled binary search for the most significant bit.
            if ((value & mask1) != 0) iPos += 16;
            mask2 <<= iPos;

            if ((value & mask2) != 0) iPos += 8;
            mask3 <<= iPos;

            if ((value & mask3) != 0) iPos += 4;
            mask4 <<= iPos;

            if ((value & mask4) != 0) iPos += 2;
            mask5 <<= iPos;

            if ((value & mask5) != 0) iPos++;

            return iPos;
        }

        /// <summary>
        /// Gets the most significant bit
        /// </summary>
        /// <param name="value">the input to search for the MSB in</param>
        /// <returns>-1 if the input was zero, the position of the MSB otherwise</returns>
        public static int GetMSB(UInt64 value)
        {
            if (value == 0) return -1;

            UInt64 mask0 = 0xffffffff00000000ul;
            UInt64 mask1 = 0xffff0000;
            UInt64 mask2 = 0xff00;
            UInt64 mask3 = 0xf0;
            UInt64 mask4 = 0xc;   //1100 in binary
            UInt64 mask5 = 0x2;   //10 in binary

            int iPos = 0;

            //Unrolled binary search for the most significant bit.
            if ((value & mask0) != 0) iPos += 32;
            mask1 <<= iPos;

            if ((value & mask1) != 0) iPos += 16;
            mask2 <<= iPos;

            if ((value & mask2) != 0) iPos += 8;
            mask3 <<= iPos;

            if ((value & mask3) != 0) iPos += 4;
            mask4 <<= iPos;

            if ((value & mask4) != 0) iPos += 2;
            mask5 <<= iPos;

            if ((value & mask5) != 0) iPos++;

            return iPos;
        }

        /// <summary>
        /// Gets the most significant bit
        /// </summary>
        /// <param name="value">the input to search for the MSB in</param>
        /// <returns>-1 if the input was zero, the position of the MSB otherwise</returns>
        public static int GetMSB(BigInt value)
        {
            int digit = value.GetMSD();
            int bit = GetMSB(value.digitArray[digit]);
            return (digit << 5) + bit;
        }

        //**************** Helper Functions for Div ********************

        /// <summary>
        /// Gets the index of the most significant digit
        /// </summary>
        /// <returns></returns>
        private int GetMSD()
        {
            for (int i = digitArray.Length - 1; i >= 0; i--)
            {
                if (digitArray[i] != 0) return i;
            }

            return 0;
        }

        /// <summary>
        /// Gets the required bitshift for the Div_32 algorithm
        /// </summary>
        /// <returns></returns>
        private int GetDivBitshift()
        {
            uint digit = digitArray[GetMSD()];
            uint mask1 = 0xffff0000;
            uint mask2 = 0xff00;
            uint mask3 = 0xf0;  
            uint mask4 = 0xc;   //1100 in binary
            uint mask5 = 0x2;   //10 in binary

            int iPos = 0;

            //Unrolled binary search for the most significant bit.
            if ((digit & mask1) != 0) iPos += 16;
            mask2 <<= iPos;

            if ((digit & mask2) != 0) iPos += 8;
            mask3 <<= iPos;
            
            if ((digit & mask3) != 0) iPos += 4;
            mask4 <<= iPos;

            if ((digit & mask4) != 0) iPos += 2;
            mask5 <<= iPos;

            if ((digit & mask5) != 0) return 30 - iPos;

            return 31 - iPos;
        }

        /// <summary>
        /// Shifts and optionally precision-extends the arguments to prepare for Div_32
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        private static int MakeSafeDiv(BigInt n1, BigInt n2)
        {
            int shift = n2.GetDivBitshift();
            int n1MSD = n1.GetMSD();

            uint temp = n1.digitArray[n1MSD];
            if (n1MSD == n1.digitArray.Length - 1 && ((temp << shift) >> shift) != n1.digitArray[n1MSD])
            {
                //Precision-extend n1 and n2 if necessary
                int digits = n1.digitArray.Length;
                n1.SetNumDigits(digits + 1);
                n2.SetNumDigits(digits + 1);
            }

            //Logical left-shift n1 and n2
            n1.LSH(shift);
            n2.LSH(shift);

            return shift;
        }

        /// <summary>
        /// Schoolbook division helper function.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="Q">Quotient output value</param>
        /// <param name="R">Remainder output value</param>
        private static void Div_31(BigInt n1, BigInt n2, BigInt Q, BigInt R)
        {
            int digitsN1 = n1.GetMSD() + 1;
            int digitsN2 = n2.GetMSD() + 1;            

            if ((digitsN1 > digitsN2))
            {
                BigInt n1New = new BigInt(n2);
                n1New.DigitShiftSelfLeft(1);

                //If n1 >= n2 * 2^32
                if (!LtInt(n1, n1New))
                {
                    n1New.sign = n1.sign;
                    SubFast(n1New, n1, n1New);

                    Div_32(n1New, n2, Q, R);

                    //Q = (A - B*2^32)/B + 2^32
                    Q.Add2Pow32Self();
                    return;
                }
            }

            UInt32 q = 0;

            if (digitsN1 >= 2)
            {
                UInt64 q64 = ((((UInt64)n1.digitArray[digitsN1 - 1]) << 32) + n1.digitArray[digitsN1 - 2]) / (UInt64)n2.digitArray[digitsN2 - 1];

                if (q64 > 0xfffffffful)
                {
                    q = 0xffffffff;
                }
                else
                {
                    q = (UInt32)q64;
                }
            }

            BigInt temp = Mul(n2, q);

            if (GtInt(temp, n1))
            {
                temp.SubInternalBits(n2.digitArray);
                q--;

                if (GtInt(temp, n1))
                {
                    temp.SubInternalBits(n2.digitArray);
                    q--;
                }
            }

            Q.Zero();
            Q.digitArray[0] = q;
            R.Assign(n1);
            R.SubInternalBits(temp.digitArray);
        }

        /// <summary>
        /// Schoolbook division algorithm
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="Q"></param>
        /// <param name="R"></param>
        private static void Div_32(BigInt n1, BigInt n2, BigInt Q, BigInt R)
        {
            int digitsN1 = n1.GetMSD() + 1;
            int digitsN2 = n2.GetMSD() + 1;

            //n2 is bigger than n1
            if (digitsN1 < digitsN2)
            {
                R.AssignInt(n1);
                Q.Zero();
                return;
            }

            if (digitsN1 == digitsN2)
            {
                //n2 is bigger than n1
                if (LtInt(n1, n2))
                {
                    R.AssignInt(n1);
                    Q.Zero();
                    return;
                }

                //n2 >= n1, but less the 2x n1 (initial conditions make this certain)
                Q.Zero();
                Q.digitArray[0] = 1;
                R.Assign(n1);
                R.SubInternalBits(n2.digitArray);
                return;
            }

            int digits = digitsN1 - (digitsN2 + 1);

            //Algorithm Div_31 can be used to get the answer in O(n) time.
            if (digits == 0)
            {
                Div_31(n1, n2, Q, R);
                return;
            }

            BigInt n1New = DigitShiftRight(n1, digits);
            BigInt s = DigitTruncate(n1, digits);

            BigInt Q2 = new BigInt(n1, n1.pres, true);
            BigInt R2 = new BigInt(n1, n1.pres, true);

            Div_31(n1New, n2, Q2, R2);

            R2.DigitShiftSelfLeft(digits);
            R2.Add(s);

            Div_32(R2, n2, Q, R);

            Q2.DigitShiftSelfLeft(digits);
            Q.Add(Q2);
        }

        /// <summary>
        /// Sets the n-th bit of the number to 1
        /// </summary>
        /// <param name="bit">the index of the bit to set</param>
        public void SetBit(int bit)
        {
            int digit = (bit >> 5);
            if (digit >= digitArray.Length) return;
            digitArray[digit] = digitArray[digit] | (1u << (bit - (digit << 5)));
        }

        /// <summary>
        /// Sets the n-th bit of the number to 0
        /// </summary>
        /// <param name="bit">the index of the bit to set</param>
        public void ClearBit(int bit)
        {
            int digit = (bit >> 5);
            if (digit >= digitArray.Length) return;
            digitArray[digit] = digitArray[digit] & (~(1u << (bit - (digit << 5))));
        }

        /// <summary>
        /// Returns the n-th bit, counting from the MSB to the LSB
        /// </summary>
        /// <param name="bit">the index of the bit to return</param>
        /// <returns>1 if the bit is 1, 0 otherwise</returns>
        public uint GetBitFromTop(int bit)
        {
            if (bit < 0) return 0;
            int wordCount = (bit >> 5);
            int upBit = 31 - (bit & 31);
            if (wordCount >= digitArray.Length) return 0;

            return ((digitArray[digitArray.Length - (wordCount + 1)] & (1u << upBit)) >> upBit);
        }

        /// <summary>
        /// Assigns n2 to 'this'
        /// </summary>
        /// <param name="n2"></param>
        public void Assign(BigInt n2)
        {
            if (digitArray.Length != n2.digitArray.Length) MakeSafe(ref n2);
            sign = n2.sign;
            AssignInt(n2);
        }

        /// <summary>
        /// Assign n2 to 'this', safe only if precision-matched
        /// </summary>
        /// <param name="n2"></param>
        /// <returns></returns>
        private void AssignInt(BigInt n2)
        {
            int Length = digitArray.Length;

            for (int i = 0; i < Length; i++)
            {
                digitArray[i] = n2.digitArray[i];
            }
        }

        private static BigInt DigitShiftRight(BigInt n1, int digits)
        {
            BigInt res = new BigInt(n1);

            int Length = res.digitArray.Length;

            for (int i = 0; i < Length - digits; i++)
            {
                res.digitArray[i] = res.digitArray[i + digits];
            }

            for (int i = Length - digits; i < Length; i++)
            {
                res.digitArray[i] = 0;
            }

            return res;
        }

        private void DigitShiftSelfRight(int digits)
        {
            for (int i = digits; i < digitArray.Length; i++)
            {
                digitArray[i - digits] = digitArray[i];
            }

            for (int i = digitArray.Length - digits; i < digitArray.Length; i++)
            {
                digitArray[i] = 0;
            }
        }

        private void DigitShiftSelfLeft(int digits)
        {
            for (int i = digitArray.Length - 1; i >= digits; i--)
            {
                digitArray[i] = digitArray[i - digits];
            }

            for (int i = digits - 1; i >= 0; i--)
            {
                digitArray[i] = 0;
            }
        }

        private static BigInt DigitTruncate(BigInt n1, int digits)
        {
            BigInt res = new BigInt(n1);

            for (int i = res.digitArray.Length - 1; i >= digits; i--)
            {
                res.digitArray[i] = 0;
            }

            return res;
        }

        private void Add2Pow32Self()
        {
            int Length = digitArray.Length;

            uint carry = 1;

            for (int i = 1; i < Length; i++)
            {
                uint temp = digitArray[i];
                digitArray[i] += carry;
                if (digitArray[i] > temp) return;
            }

            return;
        }

        /// <summary>
        /// Sets the number of digits without changing the precision
        /// This method is made public only to facilitate fixed-point operations
        /// It should under no circumstances be used for anything else, because
        /// it breaks the BigNum(PrecisionSpec precision) constructor in dangerous
        /// and unpredictable ways.
        /// </summary>
        /// <param name="digits"></param>
        public void SetNumDigits(int digits)
        {
            if (digits == digitArray.Length) return;

            UInt32[] newDigitArray = new UInt32[digits];
            workingSet = new UInt32[digits];

            int numCopy = (digits < digitArray.Length) ? digits : digitArray.Length;

            for (int i = 0; i < numCopy; i++)
            {
                newDigitArray[i] = digitArray[i];
            }

            digitArray = newDigitArray;
        }

        //********************** Explicit casts ***********************

        /// <summary>
        /// Cast to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Int32(BigInt value)
        {
            if (value.digitArray[0] == 0x80000000 && value.sign) return Int32.MinValue;
            int res = (int)(value.digitArray[0] & 0x7fffffff);
            if (value.sign) res = -res;
            return res;
        }

        /// <summary>
        /// explicit cast to unsigned int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator UInt32(BigInt value)
        {
            if (value.sign) return (UInt32)((Int32)(value));
            return (UInt32)value.digitArray[0];
        }

        /// <summary>
        /// explicit cast to 64-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Int64(BigInt value)
        {
            if (value.digitArray.Length < 2) return (value.sign ? -((Int64)value.digitArray[0]): ((Int64)value.digitArray[0]));
            UInt64 ret = (((UInt64)value.digitArray[1]) << 32) + (UInt64)value.digitArray[0];
            if (ret == 0x8000000000000000L && value.sign) return Int64.MinValue;
            Int64 signedRet = (Int64)(ret & 0x7fffffffffffffffL);
            if (value.sign) signedRet = -signedRet;
            return signedRet;
        }

        /// <summary>
        /// Explicit cast to UInt64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator UInt64(BigInt value)
        {
            if (value.sign) return (UInt64)((Int64)(value));
            if (value.digitArray.Length < 2) return (UInt64)value.digitArray[0];
            return ((((UInt64)value.digitArray[1]) << 32) + (UInt64)value.digitArray[0]);
        }

        /// <summary>
        /// Cast to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator string(BigInt value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Cast from string - this is not wholly safe, because precision is not
        /// specified. You should try to construct a BigInt with the appropriate
        /// constructor instead.
        /// </summary>
        /// <param name="value">The decimal string to convert to a BigInt</param>
        /// <returns>A BigInt of the precision required to encompass the string</returns>
        public static explicit operator BigInt(string value)
        {
            return new BigInt(value);
        }

        //********************* ToString members **********************

        /// <summary>
        /// Converts this to a string, in the specified base
        /// </summary>
        /// <param name="numberBase">the base to use (min 2, max 16)</param>
        /// <returns>a string representation of the number</returns>
        public string ToString(int numberBase)
        {
            char[] digitChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            string output = "";

            BigInt clone = new BigInt(this);
            clone.sign = false;

            int numDigits = 0;
            while (!clone.IsZero())
            {
                if (numberBase == 10 && (numDigits % 3) == 0 && numDigits != 0)
                {
                    output = String.Format(",{0}", output);
                }
                else if (numberBase != 10 && (numDigits % 8) == 0 && numDigits != 0)
                {
                    output = String.Format(" {0}", output);
                }

                BigInt div, mod;
                DivMod(clone, (uint)numberBase, out div, out mod);
                int iMod = (int)mod;
                output = String.Format("{0}{1}", digitChars[(int)mod], output);

                numDigits++;

                clone = div;
            }

            if (output.Length == 0) output = String.Format("0");

            if (sign) output = String.Format("-{0}", output);

            return output;
        }

        /// <summary>
        /// Converts the number to a string, in base 10
        /// </summary>
        /// <returns>a string representation of the number in base 10</returns>
        public override string ToString()
        {
            return ToString(10);
        }

        //***************** Internal helper functions *****************

        private void FromStringInt(string init, int numberBase)
        {
            char [] digitChars = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};

            string formattedInput = init.Trim().ToUpper();

            for (int i = 0; i < formattedInput.Length; i++)
            {
                int digitIndex = Array.IndexOf(digitChars, formattedInput[i]);

                //Skip fractional part altogether
                if (formattedInput[i] == '.') break;

                //skip non-digit characters.
                if (digitIndex < 0) continue;

                //Multiply
                MulInternal((uint)numberBase);
              
                //Add
                AddInternal(digitIndex);
            }

            if (init.Length > 0 && init[0] == '-') sign = true;
        }

        /// <summary>
        /// Sign-insensitive less than comparison. 
        /// unsafe if n1 and n2 disagree in precision
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private static bool LtInt(BigInt n1, BigInt n2)
        {
            //MakeSafe(ref n1, ref n2);

            for (int i = n1.digitArray.Length - 1; i >= 0; i--)
            {
                if (n1.digitArray[i] < n2.digitArray[i]) return true;
                if (n1.digitArray[i] > n2.digitArray[i]) return false;
            }

            //equal
            return false;
        }

        /// <summary>
        /// Sign-insensitive greater than comparison. 
        /// unsafe if n1 and n2 disagree in precision
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private static bool GtInt(BigInt n1, BigInt n2)
        {
            //MakeSafe(ref n1, ref n2);

            for (int i = n1.digitArray.Length - 1; i >= 0; i--)
            {
                if (n1.digitArray[i] > n2.digitArray[i]) return true;
                if (n1.digitArray[i] < n2.digitArray[i]) return false;
            }

            //equal
            return false;
        }

        /// <summary>
        /// Makes sure the numbers have matching precisions
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        private static void MakeSafe(ref BigInt n1, ref BigInt n2)
        {
            if (n1.digitArray.Length == n2.digitArray.Length)
            {
                return;
            }
            else if (n1.digitArray.Length > n2.digitArray.Length)
            {
                n2 = new BigInt(n2, n1.pres);
            }
            else
            {
                n1 = new BigInt(n1, n2.pres);
            }
        }

        /// <summary>
        /// Makes sure the numbers have matching precisions
        /// </summary>
        /// <param name="n2">the number to match to this</param>
        private void MakeSafe(ref BigInt n2)
        {
            n2 = new BigInt(n2, pres);
            n2.SetNumDigits(digitArray.Length);
        }


        private PrecisionSpec pres;
        private bool sign;
        private uint[] digitArray;
        private uint[] workingSet;
    }

}
