using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MEncrypt_Commonly_implementation
{
    class MEncrypter
    {
        private readonly List<byte> _keyBytes;
        const int MAX = 256;
        public string Key
        {
            get { return string.Join(string.Empty, _keyBytes.Select(_s_ => _s_.ToString("X2"))); }
        }

        static bool IsOk(IEnumerable<byte> _key_)
        {
            var key = _key_.ToList();
            return _key_ != null && key.Count == MAX && key.All(_b_ => key.Count(_bb_ => _bb_ == _b_) == 1);
        }

        public static byte[] GenerateRandomKey()
        {
            var random = new Random();
            var list = Enumerable.Range(0, MAX).Select(_s_ => (byte)_s_).ToList();
            var cvb = new byte[MAX];
            for (var i = 0; i < MAX; i++)
            {
                var r = list[random.Next(0, list.Count)];
                cvb[i] = r;
                list.Remove(r);
            }
            return cvb;
        }

        public enum StringFormat
        {
            Hex,Base64
        }

        public IEnumerable<byte> Encrypt(IEnumerable<byte> _bytes_)
        {
            return _bytes_.Select(_b_ => _keyBytes[_b_]);
        }
        public IEnumerable<byte> Decrypt(IEnumerable<byte> _bytes_)
        {
            return _bytes_.Select(_b_ => (byte) _keyBytes.IndexOf(_b_));
        }

        public MEncrypter(string _key_ ,StringFormat _format_)
        {
            if (_format_==StringFormat.Base64)
            {
                _keyBytes = Convert.FromBase64String(_key_).ToList();
                return;
            }
            var list = new List<byte>();
            var token = string.Empty;
            var i = 0;
            while (i<_key_.Length)
            {
                while (token.Length!=2)
                {
                    token += _key_[i].ToString(CultureInfo.InvariantCulture);
                    i++;
                }
                var b = Convert.ToByte(token, 16);
                list.Add(b);
                token = string.Empty;
            }
            _keyBytes = list;
        }
        public MEncrypter(IEnumerable<byte> _key_)
        {
            _key_ = _key_.ToList();
            if (!IsOk(_key_))
            {
                throw new ArgumentException("Invalid Key");
            }
            _keyBytes = _key_.ToList();
        }

        public byte[] Decrypt(byte[] _arrayOfBytes_)
        {
            var len = _arrayOfBytes_.Length;
            var result = new byte[len];
            for (var i = 0; i < len; i++)
            {
                result[i] = (byte) _keyBytes.ToList().IndexOf(_arrayOfBytes_[i]);
            }
            return result;
        }
        public byte[] Encrypt(byte[] _arrayOfBytes_)
        {
            var len = _arrayOfBytes_.Length;
            var result = new byte[len];
            for (var i = 0; i < len; i++)
            {
                result[i] = _keyBytes[_arrayOfBytes_[i]];
            }
            return result;
        }
    }
}
