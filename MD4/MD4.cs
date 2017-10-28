 using System.Linq;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class MD4 : HashAlgorithm
    {
        private uint _a;
        private uint _b;
        private uint _c;
        private uint _d;
        private readonly uint[] _x;
        private int _bytesProcessed;

        public MD4()
        {
            _x = new uint[16];
            Initialize();
        }

        public override sealed void Initialize()
        {
            _a = 0x67452301;
            _b = 0xefcdab89;
            _c = 0x98badcfe;
            _d = 0x10325476;

            _bytesProcessed = 0;
        }

        protected override void HashCore(byte[] _array_, int _offset_, int _length_)
        {
            ProcessMessage(Bytes(_array_, _offset_, _length_));
        }

        protected override byte[] HashFinal()
        {
            try
            {
                ProcessMessage(Padding());

                return new[] { _a, _b, _c, _d }.SelectMany(Bytes).ToArray();
            }
            finally
            {
                Initialize();
            }
        }

        private void ProcessMessage(IEnumerable<byte> _bytes_)
        {
            foreach (var b in _bytes_)
            {
                var c = _bytesProcessed & 63;
                var i = c >> 2;
                var s = (c & 3) << 3;

                _x[i] = (_x[i] & ~((uint)255 << s)) | ((uint)b << s);

                if (c == 63)
                {
                    Process16WordBlock();
                }

                _bytesProcessed++;
            }
        }

        private static IEnumerable<byte> Bytes(byte[] _bytes_, int _offset_, int _length_)
        {
            for (var i = _offset_; i < _length_; i++)
            {
                yield return _bytes_[i];
            }
        }

        private static IEnumerable<byte> Bytes(uint _word_)
        {
            yield return (byte)(_word_ & 255);
            yield return (byte)((_word_ >> 8) & 255);
            yield return (byte)((_word_ >> 16) & 255);
            yield return (byte)((_word_ >> 24) & 255);
        }

        private static IEnumerable<byte> Repeat(byte _value_, int _count_)
        {
            //var enumerable = Enumerable.Repeat(_value_, _count_);
            //foreach (var b in enumerable)
            //{
            //    yield return b;
            //}
            for (var i = 0; i < _count_; i++)
            {
                yield return _value_;
            }
        }

        private IEnumerable<byte> Padding()
        {
            return Repeat(128, 1)
               .Concat(Repeat(0, ((_bytesProcessed + 8) & 0x7fffffc0) + 55 - _bytesProcessed))
               .Concat(Bytes((uint)_bytesProcessed << 3))
               .Concat(Repeat(0, 4));
        }

        private void Process16WordBlock()
        {
            var aa = _a;
            var bb = _b;
            var cc = _c;
            var dd = _d;

            foreach (var k in new[] { 0, 4, 8, 12 })
            {
                aa = Round1Operation(aa, bb, cc, dd, _x[k], 3);
                dd = Round1Operation(dd, aa, bb, cc, _x[k + 1], 7);
                cc = Round1Operation(cc, dd, aa, bb, _x[k + 2], 11);
                bb = Round1Operation(bb, cc, dd, aa, _x[k + 3], 19);
            }

            foreach (var k in new[] { 0, 1, 2, 3 })
            {
                aa = Round2Operation(aa, bb, cc, dd, _x[k], 3);
                dd = Round2Operation(dd, aa, bb, cc, _x[k + 4], 5);
                cc = Round2Operation(cc, dd, aa, bb, _x[k + 8], 9);
                bb = Round2Operation(bb, cc, dd, aa, _x[k + 12], 13);
            }

            foreach (var k in new[] { 0, 2, 1, 3 })
            {
                aa = Round3Operation(aa, bb, cc, dd, _x[k], 3);
                dd = Round3Operation(dd, aa, bb, cc, _x[k + 8], 9);
                cc = Round3Operation(cc, dd, aa, bb, _x[k + 4], 11);
                bb = Round3Operation(bb, cc, dd, aa, _x[k + 12], 15);
            }

            unchecked
            {
                _a += aa;
                _b += bb;
                _c += cc;
                _d += dd;
            }
        }

        private static uint ROL(uint _value_, int _numberOfBits_)
        {
            return (_value_ << _numberOfBits_) | (_value_ >> (32 - _numberOfBits_));
        }

        private static uint Round1Operation(uint _a_, uint _b_, uint _c_, uint _d_, uint _xk_, int _s_)
        {
            unchecked
            {
                return ROL(_a_ + ((_b_ & _c_) | (~_b_ & _d_)) + _xk_, _s_);
            }
        }

        private static uint Round2Operation(uint _a_, uint _b_, uint _c_, uint _d_, uint _xk_, int _s_)
        {
            unchecked
            {
                return ROL(_a_ + ((_b_ & _c_) | (_b_ & _d_) | (_c_ & _d_)) + _xk_ + 0x5a827999, _s_);
            }
        }

        private static uint Round3Operation(uint _a_, uint _b_, uint _c_, uint _d_, uint _xk_, int _s_)
        {
            unchecked
            {
                return ROL(_a_ + (_b_ ^ _c_ ^ _d_) + _xk_ + 0x6ed9eba1, _s_);
            }
        }
    }
