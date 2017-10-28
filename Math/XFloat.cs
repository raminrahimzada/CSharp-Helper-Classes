using System;
using System.Globalization;
using System.Linq;
namespace BigFloat_Test
{
    public class XFloat : IComparable<XFloat>, IDisposable, ICloneable
    {
        #region extension method

        public static class Extensions
        {
            public static string GetSagdan(string _str_, int _shift_, out int _startIndex_, char _artirma_ = '0')
            {
                //123456
                //4
                if (_str_.Length > _shift_)
                {
                    _startIndex_ = _str_.Length - _shift_;
                    return _str_.Substring(_startIndex_);
                }
                var i = 0;
                while (_str_.Length < _shift_)
                {
                    i++;
                    _str_ = _artirma_ + _str_;
                }
                _startIndex_ = _str_.Length - _shift_ - i;
                return _str_.Substring(_startIndex_);
            }

            private static int Toint(char _ch_)
            {
                switch (_ch_)
                {
                    case '0':
                        return 0;
                    case '1':
                        return 1;
                    case '2':
                        return 2;
                    case '3':
                        return 3;
                    case '4':
                        return 4;
                    case '5':
                        return 5;
                    case '6':
                        return 6;
                    case '7':
                        return 7;
                    case '8':
                        return 8;
                    case '9':
                        return 9;
                }
                throw new Exception("not  a number ");
            }

            public static string Ferq(string _a_, string _b_)
            {
                while (_a_.Length < _b_.Length)
                {
                    _a_ = "0" + _a_;
                }
                while (_b_.Length < _a_.Length)
                {
                    _b_ = "0" + _b_;
                }
                var cvb = "";
                var len = _a_.Length - 1;
                var qaliq = 0;
                for (var i = len; i >= 0; i--)
                {
                    var i1 = Toint(_a_[i]);
                    var i2 = Toint(_b_[i]);
                    var cem = i1 - i2 - qaliq;
                    if (cem < 0)
                    {
                        cem += 10;
                        cvb = cem + cvb;
                        qaliq = 1;
                    }
                    else
                    {
                        cvb = cem + cvb;
                        qaliq = 0;
                    }
                }
                if (qaliq != 0)
                {
                    cvb = qaliq + cvb;
                }
                return cvb;
            }

            private static string Hasil(string _a_, int _x_)
            {
                if (_x_ == 0)
                {
                    return "";
                }
                if (_x_ == 1)
                {
                    return _a_;
                }
                var i = _a_.Length - 1;
                var qaliq = 0;
                var cvb = "";
                A:
                var an = Toint(_a_[i]);
                var vuruq = an*_x_ + qaliq;
                if (vuruq >= 10)
                {
                    cvb = vuruq%10 + cvb;
                    qaliq = vuruq/10;
                }
                else
                {
                    cvb = vuruq + cvb;
                    qaliq = 0;
                }
                i--;
                if (i == -1)
                {
                    if (qaliq != 0)
                    {
                        cvb = qaliq + cvb;
                    }
                    return cvb;
                }
                goto A;
            }

            public static string Hasil(string _a_, string _b_)
            {
                while (_a_.Length < _b_.Length)
                {
                    _a_ = "0" + _a_;
                }
                while (_b_.Length < _a_.Length)
                {
                    _b_ = "0" + _b_;
                }
                var cvb = "";
                string[] sonluq = {""};
                for (var i = _b_.Length - 1; i >= 0; i--)
                {
                    var ch = _b_[i];
                    var curr = Hasil(_a_, Toint(ch)) + sonluq[0];
                    cvb = Cem(cvb, curr);
                    sonluq[0] += "0";
                }
                return cvb;
            }

            public static string Cem(string _a_, string _b_)
            {
                if (_a_.All(_ch_ => _ch_ == '0'))
                {
                    return _b_;
                }
                if (_b_.All(_ch_ => _ch_ == '0'))
                {
                    return _a_;
                }
                while (_a_.Length < _b_.Length)
                {
                    _a_ = "0" + _a_;
                }
                while (_b_.Length < _a_.Length)
                {
                    _b_ = "0" + _b_;
                }
                var cvb = "";
                var len = _a_.Length - 1;
                var qaliq = 0;
                for (var i = len; i >= 0; i--)
                {
                    var i1 = Toint(_a_[i]);
                    var i2 = Toint(_b_[i]);
                    var cem = i1 + i2 + qaliq;
                    if (cem >= 10)
                    {
                        cvb = cem%10 + cvb;
                        qaliq = cem/10;
                    }
                    else
                    {
                        cvb = cem + cvb;
                        qaliq = 0;
                    }
                }
                if (qaliq != 0)
                {
                    cvb = qaliq + cvb;
                }
                return cvb;
            }

            public static int CompareAla(string _a_, string _b_)
            {
                //1234
                //4687
                _a_ = _a_.TrimStart('0');
                _b_ = _b_.TrimStart('0');
                var lena = _a_.Length;
                var lenb = _b_.Length;
                if (lena > lenb)
                {
                    return 1;
                }
                if (lena < lenb)
                {
                    return -1;
                }
                for (var i = 0; i < lena; i++)
                {
                    if (Toint(_a_[i]) > Toint(_b_[i]))
                    {
                        return 1;
                    }
                    if (Toint(_a_[i]) < Toint(_b_[i]))
                    {
                        return -1;
                    }
                }
                return 0;
            }

            public static void IxtisarZero(ref string _a_, ref string _b_)
            {
                EvveldenAxirdanEyniSifirlariSil(ref _a_, ref _b_);
                AxirdenAxirdanEyniSifirlariSil(ref _a_, ref _b_);
            }

            public static int AxirindekiSifirSayi(string _s_)
            {
                var i = 0;
                for (var index = _s_.Length - 1; index >= 0; index--)
                {
                    var t = _s_[index];
                    if (t != '0')
                    {
                        return i;
                    }
                    i++;
                }
                return i;
            }

            public static int EvvelindekiSifirSayi(string _s_)
            {
                var i = 0;
                foreach (var t in _s_)
                {
                    if (t != '0')
                    {
                        return i;
                    }
                    i++;
                }
                return i;
            }

            public static void EvveldenAxirdanEyniSifirlariSil(ref string _a_, ref string _b_)
            {
                var son0A = AxirindekiSifirSayi(_a_);
                var son0B = AxirindekiSifirSayi(_b_);
                var min0 = son0A < son0B ? son0A : son0B;
                _a_ = _a_.Substring(0, _a_.Length - min0);
                _b_ = _b_.Substring(0, _b_.Length - min0);
            }

            public static void AxirdenAxirdanEyniSifirlariSil(ref string _a_, ref string _b_)
            {
                var son0A = EvvelindekiSifirSayi(_a_);
                var son0B = EvvelindekiSifirSayi(_b_);
                var min0 = son0A < son0B ? son0A : son0B;
                _a_ = _a_.Substring(min0);
                _b_ = _b_.Substring(min0);
            }

            public static string BolmeTam(string _a_, string _b_)
            {
                IxtisarZero(ref _a_, ref _b_);
                _a_ = _a_.TrimStart('0');
                _b_ = _b_.TrimStart('0');
                if (_b_.TrimStart('0') == "1")
                {
                    return _a_;
                }
                if (_b_.All(_ch_ => _ch_ == '0'))
                {
                    throw new Exception("divide by 0");
                }
                /*
                 * 12345|97
                 * 
                 */
                var lena = _a_.Length;
                var lenb = _b_.Length;
                if (lena < lenb)
                {
                    return "0";
                }
                if (CompareAla(_a_, _b_) == -1)
                {
                    return "0";
                }
                var cvb = "";
                var cur = _a_.Substring(0, lenb);
                var i = 0;

                a:
                var digit = 0;
                var ok = false;
                while (CompareAla(cur, _b_) == -1)
                {
                    var ind = lenb + i++;
                    if (ind == lena)
                    {
                        cvb = cvb + digit;
                        return cvb;
                    }
                    cur += _a_[ind].ToString(CultureInfo.InvariantCulture);
                    if (ok) cvb += "0";
                    ok = true;
                }
                while (CompareAla(cur, _b_) != -1)
                {
                    digit++;
                    cur = Ferq(cur, _b_);
                }
                cvb += digit;
                if (lenb + i < _a_.Length)
                {
                    goto a;
                }

                return cvb;
            }
        }

        #endregion

        #region Methods

        #region Normal

        #region Public method

        #region override public methods

        public override string ToString()
        {
            var ta = Tam.TrimStart('0');
            if (ta == "")
            {
                ta = "0";
            }
            var ke = Kesr.TrimEnd('0');
            if (ke == "")
            {
                return (Sign == -1 ? "-" : "") + ta;
            }
            return (Sign == -1 ? "-" : "") + ta + NOQTE + ke;
        }

        #endregion

        #region Ordinary

        public string ToStringFull()
        {
            return (Sign == -1 ? "-" : "") + Tam + NOQTE + Kesr;
        }

        public void Dispose()
        {
            Tam = null;
            Kesr = null;
        }

        public bool IsZero
        {
            get { return Tam.All(_ch_ => _ch_ == '0') && Kesr.All(_ch_ => _ch_ == '0'); }
        }

        #endregion

        #region From interfaces

        public int CompareTo(XFloat _other_)
        {
            var signA = _other_.Sign;
            var signT = Sign;
            if (signA == -1 && signT == 1)
            {
                return 1;
            }
            if (signA == 1 && signT == -1)
            {
                return -1;
            }
            if (signA == 1 && signT == 1)
            {
                var tamC = Extensions.CompareAla(Tam, _other_.Tam);
                if (tamC != 0)
                {
                    return tamC;
                }
                var kesrO = _other_.Kesr;
                var kesrT = Kesr;
                tamC = Extensions.CompareAla(kesrT, kesrO);
                return tamC;
            }
                //ikisi de menfidi
            else
            {
                var tamC = Extensions.CompareAla(Tam, _other_.Tam);
                if (tamC != 0)
                {
                    return -tamC;
                }
                var kesrO = _other_.Kesr;
                var kesrT = Kesr;
                tamC = Extensions.CompareAla(kesrT, kesrO);
                return -tamC;
            }
            // < -1
            // > 1
        }

        public object Clone()
        {
            return new XFloat(ToString());
        }

        #endregion

        #region operators

        #region operator +

        #region operator x+?

        public static XFloat operator +(XFloat _a_, XFloat _b_)
        {
            return Sum(_a_, _b_);
        }

        public static XFloat operator +(XFloat _a_, int _b_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(XFloat _a_, long _b_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(XFloat _a_, ulong _b_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(XFloat _a_, double _b_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(XFloat _a_, float _b_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(XFloat _a_, byte _b_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        #endregion

        #region operator ?+x

        public static XFloat operator +(int _b_, XFloat _a_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(long _b_, XFloat _a_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(ulong _b_, XFloat _a_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(double _b_, XFloat _a_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(float _b_, XFloat _a_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        public static XFloat operator +(byte _b_, XFloat _a_)
        {
            return Sum(_a_, new XFloat(_b_));
        }

        #endregion

        #endregion

        #region operator -

        #region x-?

        public static XFloat operator -(XFloat _a_, XFloat _b_)
        {
            return Substract(_a_, _b_);
        }

        public static XFloat operator -(XFloat _a_, int _b_)
        {
            return Substract(_a_, new XFloat(_b_));
        }

        public static XFloat operator -(XFloat _a_, long _b_)
        {
            return Substract(_a_, new XFloat(_b_));
        }

        public static XFloat operator -(XFloat _a_, ulong _b_)
        {
            return Substract(_a_, new XFloat(_b_));
        }

        public static XFloat operator -(XFloat _a_, float _b_)
        {
            return Substract(_a_, new XFloat(_b_));
        }

        public static XFloat operator -(XFloat _a_, double _b_)
        {
            return Substract(_a_, new XFloat(_b_));
        }

        public static XFloat operator -(XFloat _a_, byte _b_)
        {
            return Substract(_a_, new XFloat(_b_));
        }

        #endregion

        #region ?-x

        public static XFloat operator -(int _a_, XFloat _b_)
        {
            return Substract(new XFloat(_a_), _b_);
        }

        public static XFloat operator -(long _a_, XFloat _b_)
        {
            return Substract(new XFloat(_a_), _b_);
        }

        public static XFloat operator -(ulong _a_, XFloat _b_)
        {
            return Substract(new XFloat(_a_), _b_);
        }

        public static XFloat operator -(float _a_, XFloat _b_)
        {
            return Substract(new XFloat(_a_), _b_);
        }

        public static XFloat operator -(double _a_, XFloat _b_)
        {
            return Substract(new XFloat(_a_), _b_);
        }

        public static XFloat operator -(byte _a_, XFloat _b_)
        {
            return Substract(new XFloat(_a_), _b_);
        }

        #endregion

        #endregion

        #region operator *

        #region x*?

        public static XFloat operator *(XFloat _a_, XFloat _b_)
        {
            return Multiply(_a_, _b_);
        }

        public static XFloat operator *(XFloat _a_, int _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(XFloat _a_, long _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(XFloat _a_, ulong _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(XFloat _a_, short _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(XFloat _a_, double _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(XFloat _a_, float _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(XFloat _a_, byte _b_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        #endregion

        #region ?*X

        public static XFloat operator *(int _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(long _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(ulong _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(short _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(double _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(float _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        public static XFloat operator *(byte _b_, XFloat _a_)
        {
            return Multiply(_a_, (XFloat) _b_);
        }

        #endregion

        #endregion

        #region operator /

        #region x/?

        public static XFloat operator /(XFloat _a_, XFloat _b_)
        {
            return Divide(_a_, _b_);
        }

        public static XFloat operator /(XFloat _a_, int _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        public static XFloat operator /(XFloat _a_, byte _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        public static XFloat operator /(XFloat _a_, short _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        public static XFloat operator /(XFloat _a_, long _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        public static XFloat operator /(XFloat _a_, ulong _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        public static XFloat operator /(XFloat _a_, float _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        public static XFloat operator /(XFloat _a_, double _b_)
        {
            return Divide(_a_, (XFloat) _b_);
        }

        #endregion

        #region ?/x

        public static XFloat operator /(int _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        public static XFloat operator /(byte _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        public static XFloat operator /(short _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        public static XFloat operator /(long _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        public static XFloat operator /(ulong _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        public static XFloat operator /(float _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        public static XFloat operator /(double _a_, XFloat _b_)
        {
            return Divide((XFloat) _a_, _b_);
        }

        #endregion

        #endregion

        #region operator explicit

        public static explicit operator XFloat(int _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(byte _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(short _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(long _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(ulong _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(float _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(double _i_)
        {
            return new XFloat(_i_);
        }

        public static explicit operator XFloat(string _i_)
        {
            return new XFloat(_i_);
        }

        #endregion

        #endregion

        #endregion

        #region private Normal methods

        private string ToStringNoSign()
        {
            return Tam + NOQTE + Kesr;
        }

        private string Menfi()
        {
            var t = (Sign == 1 ? "-" : "") + ToStringNoSign();
            return t;
        }

        #endregion

        #endregion

        #region static

        #region Private static methods

        private static XFloat CemMusbet(XFloat _a_, XFloat _b_)
        {
            // a=12.56
            // a=34.78
            // tamlarin cemi=46
            // kesrlerin cemi=134
            var aTam = _a_.Tam;
            var bTam = _b_.Tam;
            var aKesr = _a_.Kesr;
            var bKesr = _b_.Kesr;
            var tamlar = Extensions.Cem(aTam, bTam);
            var kesrler = Extensions.Cem(aKesr, bKesr);
            int startIndex;
            var kesrlerKesr = Extensions.GetSagdan(kesrler, M, out startIndex);
            var kesrlerTam = kesrler.Substring(0, startIndex);
            if (kesrlerTam == "")
            {
                kesrlerTam = "0";
            }
            var yekuntam = Extensions.Cem(tamlar, kesrlerTam);
            return new XFloat(yekuntam + NOQTE + kesrlerKesr);
        }

        private static XFloat FerqMusbet(XFloat _a_, XFloat _b_)
        {
            //45.67
            //12.34
            var aTam = _a_.Tam;
            var bTam = _b_.Tam;
            var aKesr = _a_.Kesr;
            var bKesr = _b_.Kesr;
            var cmpTam = Extensions.CompareAla(aTam, bTam);
            var cmpKesr = Extensions.CompareAla(aKesr, bKesr);
            if (cmpTam == 0 && cmpKesr == 0)
            {
                return new XFloat("0");
            }
            if (_b_.Sign == 0)
            {
                return _a_.Clone() as XFloat;
            }
            if (_a_.Sign == 0)
            {
                return _b_.Menfi().Clone() as XFloat;
            }
            if (cmpTam == 1)
            {
                // cvb musbet olacaq
                var kesrFerq = Extensions.Ferq("1" + aKesr, "0" + bKesr);
                var tamFerq = Extensions.Ferq(Extensions.Ferq(aTam, "1"), bTam);
                //23.67
                //12.45
                //10                
                //122
                int startIndex;
                var kesrferqTam = Extensions.GetSagdan(kesrFerq, M, out startIndex);
                var kesrdenqalan = kesrFerq.Substring(0, startIndex);
                var yekunferqTam = Extensions.Cem(tamFerq, kesrdenqalan);
                var cvb = yekunferqTam + NOQTE + kesrferqTam;
                return new XFloat(cvb);
            }
            if (cmpTam == -1)
            {
                // cvb menfi olacaq
                return new XFloat(FerqMusbet(_b_, _a_).Menfi());
            }
            if (cmpTam != 0) throw new Exception("xeta errr");
            if (cmpKesr == 1)
            {
                // cvb musbet olacaq
                // tam hisseler beraberdi birincinin kesr hissesi ikincinnen boyukdu
                var kesrFerq = Extensions.Ferq(aKesr, bKesr);
                return new XFloat("0." + kesrFerq);
            }
            if (cmpKesr == -1)
            {
                // cvb menfi olacaq
                return new XFloat(FerqMusbet(_b_, _a_).Menfi());
            }
            if (cmpKesr == 0)
            {
                // eyni ededdiler
                return new XFloat("0");
            }
            throw new Exception("xeta errr");
        }

        private static XFloat Sum1(XFloat _a_, XFloat _b_)
        {
            var signA = _a_.Sign;
            var signB = _b_.Sign;
            if (signA == 0 && signB == 0)
            {
                return new XFloat("0");
            }
            if (signA == 0)
            {
                return _b_.Clone() as XFloat;
            }
            if (signB == 0)
            {
                return _a_.Clone() as XFloat;
            }
            if (signA == 1 && signB == 1)
            {
                return CemMusbet(_a_, _b_);
            }
            if (signA == 1 && signB == -1)
            {
                return FerqMusbet(_a_, new XFloat(_b_.Menfi()));
            }
            if (signA == -1 && signB == 1)
            {
                return FerqMusbet(_b_, new XFloat(_a_.Menfi()));
            }
            if (signA == -1 && signB == -1)
            {
                return new XFloat(CemMusbet(new XFloat(_a_.Menfi()), new XFloat(_b_.Menfi())).Menfi());
            }
            throw new Exception("");
        }

        private static XFloat MusbetNisbet(XFloat _a_, XFloat _b_)
        {

            /*
             * 912.34|007.45
             * ______|____
             * 
             */
            var fullA = _a_.Tam + _a_.Kesr;
            var fullB = _b_.Tam + _b_.Kesr;
            var cvb = "";
            var cur = Extensions.BolmeTam(fullA, fullB);
            if (cur.Length > N)
            {
                throw new Exception("bolme zamani cox boyuk eded alinir");
            }
            cvb += cur + NOQTE;
            var hasil = Extensions.Hasil(cur, fullB);
            fullA = Extensions.Ferq(fullA, hasil);
            // kecirux kesr hisseye
            var kesr = "";
            for (var k = 0; k < M; k++)
            {
                fullA = fullA + "0";
                fullA = fullA.TrimStart('0');
                fullB = fullB.TrimStart('0');
                cur = Extensions.BolmeTam(fullA, fullB);
                hasil = Extensions.Hasil(cur, fullB);
                fullA = Extensions.Ferq(fullA, hasil);
                cur = cur.TrimStart('0');
                if (cur == "")
                {
                    cur = "0";
                }
                kesr += cur;
            }
            if (kesr.Length > M)
            {
                kesr = kesr.Substring(0, M);
            }
            cvb += kesr;
            return new XFloat(cvb);
        }

        #endregion

        #region Public static methods

        public static XFloat Sum(params XFloat[] _aa_)
        {
            if (_aa_.Length == 0)
            {
                return Zero;
            }
            return _aa_.Length == 1 ? Sum1(_aa_[0], _aa_[1]) : _aa_.Aggregate(Zero, Sum1);
        }

        public static XFloat Substract(XFloat _a_, XFloat _b_)
        {
            var signA = _a_.Sign;
            var signB = _b_.Sign;
            if (signA == 0 && signB == 0)
            {
                return new XFloat("0");
            }
            if (signA == 0)
            {
                return new XFloat(_b_.Menfi());
            }
            if (signB == 0)
            {
                return _a_.Clone() as XFloat;
            }
            if (signA == 1 && signB == 1)
            {
                return FerqMusbet(_a_, _b_);
            }
            if (signA == 1 && signB == -1)
            {
                return CemMusbet(_a_, new XFloat(_b_.Menfi()));
            }
            if (signA == -1 && signB == 1)
            {
                return new XFloat(CemMusbet(new XFloat(_a_.Menfi()), _b_).Menfi());
            }
            if (signA == -1 && signB == -1)
            {
                return FerqMusbet(new XFloat(_b_.Menfi()), new XFloat(_a_.Menfi()));
            }
            throw new Exception("");
        }

        public static XFloat Multiply(XFloat _a_, XFloat _b_)
        {
            var aTam = _a_.Tam;
            var bTam = _b_.Tam;
            var aKesr = _a_.Kesr;
            var bKesr = _b_.Kesr;
            var aSign = _a_.Sign;
            var bSign = _b_.Sign;
            if (aSign*bSign == 0)
            {
                return new XFloat("0");
            }
            //12.34*45.67=563.5678
            var vuruq1 = aTam + aKesr;
            var vuruq2 = bTam + bKesr;
            //vuruq1 = ObsiSifirlariSil(vuruq1);
            //vuruq2 = ObsiSifirlariSil(vuruq2);
            var hasil = Extensions.Hasil(vuruq1, vuruq2);
            var kv = N + M;
            kv *= 2;
            while (hasil.Length <= (kv))
            {
                hasil = "0" + hasil;
            }
            var noqteid = hasil.Length - 2*M;
            var yekunTam = hasil.Substring(0, noqteid);
            yekunTam = yekunTam.TrimStart('0');
            var isare = aSign*bSign;
            var yekunKesr = hasil.Substring(noqteid);
            yekunKesr = yekunKesr.TrimEnd('0');
            if (yekunKesr.Length >= M)
            {
                yekunKesr = yekunKesr.Substring(0, M);
            }
            if (yekunTam == "")
            {
                yekunTam = "0";
            }
            return new XFloat((isare == -1 ? "-" : "") + yekunTam + NOQTE + yekunKesr);
        }

        public static XFloat Divide(XFloat _a_, XFloat _b_)
        {
            var signA = _a_.Sign;
            var signB = _b_.Sign;
            if (signB == 0)
            {
                throw new DivideByZeroException("");
            }
            if (signA == 0)
            {
                return Zero;
            }
            var hasil = signB*signB;
            if (hasil == -1)
            {
                var cvb = MusbetNisbet(_a_, _b_);
                return new XFloat(cvb.Menfi());
            }
            if (hasil == 1)
            {
                var cvb = MusbetNisbet(_a_, _b_);
                return cvb;
            }
            // bura hec gelmeyecek
            throw new Exception("");
        }

        #endregion

        #endregion

        #endregion

        #region Consts

        public const char NOQTE = '.';
        public const char MENFI = '-';

        #endregion

        #region Fields

        #region Normal

        #region Public Fields

        public int Sign
        {
            get { return _sign; }
            private set
            {
                switch (value)
                {
                    case 0:
                        _sign = 0;
                        break;
                    case 1:
                        _sign = 1;
                        break;
                    case -1:
                        _sign = -1;
                        break;
                    default:
                        throw new InvalidOperationException("isareye guc qiymetmenimsedildi");
                }
            }
        }

        public string Tam
        {
            get { return _tam; }
            private set
            {
                var val = value;
                while (val.Length < N)
                {
                    val = "0" + val;
                }
                if (val.Length > N)
                {
                    throw new OverflowException("tam hissede Dasma oldu");
                }
                _tam = val;
            }
        }

        public string Kesr
        {
            get { return _kesr; }
            private set
            {
                var val = value;
                while (val.Length < M)
                {
                    val = val + "0";
                }
                if (val.Length > M)
                {
                    throw new OverflowException("kesr hissede Dasma oldu");
                }
                _kesr = val;
            }
        }

        #endregion

        #region Normal fields

        private string _tam;
        private string _kesr;

        private int _sign;

        #endregion

        #endregion

        #region Static

        #region Public static Fields

        public static int N
        {
            get { return _n; }
            set
            {
                if (value > 1)
                {
                    _n = value;
                }
            }
        } // tam hissenin uzunlugu

        public static int M
        {
            get { return _m; }
            set
            {
                if (value > 1)
                {
                    _m = value;
                }
            }
        } // kesr hissenin uzunlugu

        public static XFloat Zero
        {
            get { return new XFloat("0"); }
        }

        public static XFloat One
        {
            get { return new XFloat("1"); }
        }

        public static XFloat MinusOne
        {
            get { return new XFloat("-1"); }
        }

        public static CultureInfo Culture { get; set; }

        #endregion

        #region private static fields

        private static int _n;
        private static int _m;

        #endregion

        #endregion

        #endregion

        #region Constructors

        #region Normal   Constructors

        public XFloat(string _number_)
        {
            var id = _number_.IndexOf(NOQTE);
            Sign = 1;
            if (id > 0)
            {
                var menfi = _number_[0] == MENFI;
                Tam = menfi ? _number_.Substring(1, id - 1) : _number_.Substring(0, id);
                Sign = menfi ? -1 : 1;
                Kesr = _number_.Substring(id + 1);
            }
            else
            {
                var menfi = _number_.IndexOf(MENFI);
                if (menfi >= 0) Sign = -1;
                //menfi = menfi < 0 ? 0 : menfi;
                Tam = _number_.Substring(menfi + 1);
                Kesr = "0";
            }
            if (IsZero)
            {
                Sign = 0;
            }
        }

        public XFloat(int _i_)
            : this(_i_.ToString(Culture))
        {

        }

        public XFloat(long _i_)
            : this(_i_.ToString(Culture))
        {

        }

        public XFloat(ulong _i_)
            : this(_i_.ToString(Culture))
        {

        }

        public XFloat(byte _i_)
            : this(_i_.ToString(Culture))
        {

        }

        public XFloat(short _i_)
            : this(_i_.ToString(Culture))
        {

        }

        public XFloat(double _i_)
            : this(_i_.ToString(Culture))
        {

        }

        public XFloat(float _i_)
            : this(_i_.ToString(Culture))
        {

        }

        #endregion

        #region static Constructors

        static XFloat()
        {
            N = 10;
            M = 10;
            Culture = CultureInfo.CurrentCulture;
        }

        #endregion

        #endregion
    }
}
