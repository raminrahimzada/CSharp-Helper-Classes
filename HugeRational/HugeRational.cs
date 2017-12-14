using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace HugeRational_ededler_by_Mathman
{
    class HyperFloat
    {
        #region Xəta klası
        [Serializable]
        public class HyperFloatException : Exception
        {
            public HyperFloatException() { }
            public HyperFloatException(string message) : base(message) { }
            public HyperFloatException(string message, Exception inner) : base(message, inner) { }
            protected HyperFloatException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
        #endregion

        #region dəyişənlər
        BigInteger suret;
        BigInteger mexrec;
        static int deqiqlik = 47;
        public static int Dəqiqlik
        {
            get { return deqiqlik; }
            set
            {
                if (value > 0)
                {
                    deqiqlik = value;
                }
                else
                {
                    throw new HyperFloatException("Ədədin dəqiqliyi müsbət tam ədəd olmalıdır");
                }
            }
        }
        public static HyperFloat Epsilon
        {
            get { return new HyperFloat("0." + new string('0', deqiqlik - 5) + "1"); }
        }
        #endregion

        #region Propertilər
        /// <summary>
        /// Ədədin işarəsi-ədəd mənfi olduqda -1 müsbət olduqda 1 sıfır olduqda 0 alan funksiya
        /// </summary>
        public int İşarə
        {
            get
            {
                if (suret == 0)
                {
                    return 0;
                }
                if (suret > 0)
                {
                    return 1;
                }
                return -1;
            }
        }
        /// <summary>
        /// Kəsrin surəti
        /// </summary>
        public BigInteger Surət
        {
            get { return suret; }
            set { suret = value; }
        }
        /// <summary>
        /// Kəsrin məxrəci
        /// </summary>
        public BigInteger Məxrəc
        {
            get { return mexrec; }
            set
            {
                if (mexrec == 0)
                {
                    throw new HyperFloatException("Sıfıra bölmə xətası oldu");
                }
                else
                {
                    if (mexrec < 0)
                    {
                        Surət *= -1;
                        mexrec = -value;
                    }
                }
            }
        }
        #endregion

        #region sabitler
        /// <summary>
        /// Məlum riyazi sabit olan e ədədi
        /// </summary>
        public static HyperFloat E
        {
            get { return new HyperFloat("2.7182818284590452353602874713526624977572470936999595749669676277240766303535475945713821785251664274274663919320030599218174135966290435729003342952605956307381323286279434907632338298807531952510190115738341879307021540891499348841675092447614606680822648001684774118537423454424371075390777449920695517027618386062613313845830007520449338265602976067371132007093287091274437470472306969772093101416928368190255151086574637721112523897844250569536967707854499699679468644549059879316368892300987931277361782154249992295763514822082698951936680331825288693984964651058209392398294887933203625094431173012381970684161403970198376793206832823764648042953118023287825098194558153017567173613320698112509961818815930416903515988885193458072738667385894228792284998920868058257492796104841984443634632449684875602336248270419786232090021609902353043699418491463140934317381436405462531520961836908887070167683964243781405927145635490613031072085103837505101157477041718986106873969655212671546889570328"); }
        }
        /// <summary>
        /// Məlum riyazi sabit olan π ədədi
        /// </summary>
        public static HyperFloat PI
        {
            get { return new HyperFloat("3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491298336733624406566430860213949463952247371907021798609437027705392171762931767523846748184676694051320005681271452635608277857713427577896091736371787214684409012249534301465495853710507922796892589235420199561121290219608640344181598136297747713099605187072113499999983729780499510597317328160963185950244594553469083026425223082533446850352619311881710100031378387528865875332083814206171776691473035982534904287554687311595628638823537875937519577818577805321712268066130019278766111959092164201989"); }
        }
        #endregion

        #region kostruktorlar
        /// <summary>
        /// Öz qiymətini int tipli ədəddən götürür
        /// </summary>
        /// <param name="ədəd">Hyperfloat tipinə mənimsədiləcək int tipli qiymət</param>
        public HyperFloat(int ədəd)
        {
            suret = ədəd;
            mexrec = 1;
        }
        /// <summary>
        /// Öz qiymətini long tipli ədəddən götürür
        /// </summary>
        /// <param name="ədəd">Hyperfloat tipinə mənimsədiləcək long tipli qiymət</param>
        public HyperFloat(long ədəd)
        {
            suret = ədəd;
            mexrec = 1;
        }
        /// <summary>
        /// Öz qiymətini ulong tipli ədəddən götürür
        /// </summary>
        /// <param name="ədəd">Hyperfloat tipinə mənimsədiləcək ulong tipli qiymət</param>
        public HyperFloat(ulong ədəd)
        {
            suret = ədəd;
            mexrec = 1;
        }
        /// <summary>
        /// Öz qiymətini double tipli ədəddən götürür
        /// </summary>
        /// <param name="ədəd">Hyperfloat tipinə mənimsədiləcək double tipli qiymət</param>
        public HyperFloat(double ədəd)
        {
            string eded_str = ədəd.ToString();
            int isare = 1;
            if (eded_str[0] == '-')
            {
                isare = -1;
                eded_str = eded_str.Substring(1, eded_str.Length - 1);
            }
            if (eded_str.Contains('.'))
            {
                int uz = eded_str.Length;
                int noq = eded_str.IndexOf('.');
                int ferq = uz - noq - 1;
                mexrec = BigInteger.Parse("1" + new string('0', ferq));
                var zad = eded_str.Replace(".", "");
                suret = isare * BigInteger.Parse(zad);
            }
            else
            {
                suret = isare * BigInteger.Parse(eded_str);
                mexrec = 1;
            }
            Yoxla();
        }
        /// <summary>
        /// Öz qiymətini sətr olaraq verilmiş istənilən tam və ya kəsr ədəddən götürür
        /// </summary>
        /// <param name="eded">Hyperfloat tipinə mənimsədiləcək string tipli qiymət</param>
        public HyperFloat(string ədəd)
        {
            if (ədəd.Length < 1)
            {
                throw new HyperFloatException("Heç bir ədəd tapılmadı");
            }
            int isare = 1;
            if (ədəd[0] == '-')
            {
                isare = -1;
                ədəd = ədəd.Substring(1, ədəd.Length - 1);
            }
            if (ədəd.Contains('.'))
            {
                int uz = ədəd.ToString().Length;
                int noq = ədəd.IndexOf('.');
                int ferq = uz - noq - 1;
                mexrec = BigInteger.Parse("1" + new string('0', ferq));
                var zad = ədəd.Replace(".", "");
                suret = isare * BigInteger.Parse(zad);
            }
            else
            {
                suret = isare * BigInteger.Parse(ədəd);
                mexrec = 1;
            }
            Yoxla();
        }
        /// <summary>
        /// Öz qiymətini BigInteger tipli ədəddən götürür
        /// </summary>
        /// <param name="eded">Hyperfloat tipinə mənimsədiləcək BigInteger tipli qiymət</param>
        public HyperFloat(BigInteger ədəd)
        {
            suret = ədəd;
            mexrec = 1;
        }
        /// <summary>
        /// Öz qiymətini komponentləri int tipində olan  adi kəsrdən götürür
        /// </summary>
        /// <param name="surət">Mənimsədiləcək adi kəsrin surəti</param>
        /// <param name="məxrəc">Mənimsədiləcək adi kəsrin məxrəci</param>
        public HyperFloat(int surət, int məxrəc)
        {
            if (məxrəc < 0)
            {
                surət *= (-1);
                məxrəc *= (-1);
            }
            if (məxrəc == 0)
            {
                throw new HyperFloatException("Sıfıra bölmə xətası var");
            }
            this.suret = surət;
            this.mexrec = məxrəc;
            Yoxla();
        }
        /// <summary>
        /// Öz qiymətini komponentləri long tipində olan  adi kəsrdən götürür
        /// </summary>
        /// <param name="surət">Mənimsədiləcək adi kəsrin surəti</param>
        /// <param name="məxrəc">Mənimsədiləcək adi kəsrin məxrəci</param>
        public HyperFloat(long surət, long məxrəc)
        {
            if (məxrəc < 0)
            {
                surət *= (-1);
                məxrəc *= (-1);
            }
            if (məxrəc == 0)
            {
                throw new HyperFloatException("Sıfıra bölmə xətası var");
            }
            this.suret = surət;
            this.mexrec = məxrəc;
            Yoxla();
        }
        /// <summary>
        /// Öz qiymətini komponentləri BigInteger tipində olan  adi kəsrdən götürür
        /// </summary>
        /// <param name="surət">Mənimsədiləcək adi kəsrin surəti</param>
        /// <param name="məxrəc">Mənimsədiləcək adi kəsrin məxrəci</param>
        public HyperFloat(BigInteger surət, BigInteger məxrəc)
        {
            if (məxrəc < 0)
            {
                surət *= (-1);
                məxrəc *= (-1);
            }
            if (məxrəc == 0)
            {
                throw new HyperFloatException("Sıfıra bölmə xətası var");
            }
            this.suret = surət;
            this.mexrec = məxrəc;
            Yoxla();
        }
        /// <summary>
        /// Öz qiymətini komponentləri string tipində olan  adi kəsrdən götürür
        /// </summary>
        /// <param name="surət">Mənimsədiləcək adi kəsrin surəti</param>
        /// <param name="məxrəc">Mənimsədiləcək adi kəsrin məxrəci</param>
        public HyperFloat(string surət, string məxrəc)
        {
            this.suret = BigInteger.Parse(surət);
            this.mexrec = BigInteger.Parse(məxrəc);
            if (this.mexrec < 0)
            {
                this.suret *= (-1);
                this.mexrec *= (-1);
            }
            if (this.mexrec == 0)
            {
                throw new HyperFloatException("Sıfıra bölmə xətası var");
            }
            Yoxla();
        }
        /// <summary>
        /// Başlangıc olaraq ədədə sıfır veriləcək
        /// </summary>
        public HyperFloat()
        {
            suret = 0;
            mexrec = 1;
        }
        #endregion

        #region toplama və onun override metodları
        /// <summary>
        /// İki HyperFloat tipli ədədi toplayır
        /// </summary>
        /// <param name="eded1">Birinci ədəd</param>
        /// <param name="eded2">İkinci ədəd</param>
        /// <returns>və nəticəni geri qaytarır</returns>
        public static HyperFloat Topla(HyperFloat ədəd1, HyperFloat ədəd2)
        {
            var a = ədəd1.suret;
            var c = ədəd2.suret;
            var b = ədəd1.mexrec;
            var d = ədəd2.mexrec;
            var cvb_s = a * d + b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// HyperFloat tipli və int tipli ədədləri toplayır
        /// </summary>
        /// <param name="eded1">Birinci ədəd</param>
        /// <param name="eded2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(HyperFloat ədəd1, int ədəd2)
        {
            var a = ədəd1.suret;
            var b = ədəd1.mexrec;
            var cvb_s = a + ədəd2 * b;
            var cvb_m = b;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// HyperFloat tipli və string tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(HyperFloat ədəd1, string ədəd2)
        {
            HyperFloat eded2 = new HyperFloat(ədəd2);
            var a = ədəd1.suret;
            var c = eded2.suret;
            var b = ədəd1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d + b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// HyperFloat tipli və long tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(HyperFloat ədəd1, long ədəd2)
        {
            HyperFloat eded2 = new HyperFloat(ədəd2);
            var a = ədəd1.suret;
            var c = eded2.suret;
            var b = ədəd1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d + b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// HyperFloat tipli və ulong tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(HyperFloat ədəd1, ulong ədəd2)
        {
            HyperFloat eded2 = new HyperFloat(ədəd2);
            var a = ədəd1.suret;
            var c = eded2.suret;
            var b = ədəd1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d + b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// HyperFloat tipli və double tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(HyperFloat eded1, double ədəd2)
        {
            HyperFloat eded2 = new HyperFloat(ədəd2);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d + b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// HyperFloat tipli və BigInteger tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(HyperFloat ədəd1, BigInteger ədəd2)
        {
            HyperFloat eded2 = new HyperFloat(ədəd2);
            var a = ədəd1.suret;
            var c = eded2.suret;
            var b = ədəd1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d + b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        /// <summary>
        /// int tipli və HyperFloat tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(int ədəd1, HyperFloat ədəd2)
        {
            return Topla(ədəd2, ədəd1);
        }
        /// <summary>
        /// long tipli və HyperFloat tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(long ədəd1, HyperFloat ədəd2)
        {
            return Topla(ədəd2, ədəd1);
        }
        /// <summary>
        /// long tipli və HyperFloat tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(string ədəd1, HyperFloat ədəd2)
        {
            return Topla(ədəd2, ədəd1);
        }
        /// <summary>
        /// ulong tipli və HyperFloat tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(ulong ədəd1, HyperFloat ədəd2)
        {
            return Topla(ədəd2, ədəd1);
        }
        /// <summary>
        /// double tipli və HyperFloat tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(double ədəd1, HyperFloat ədəd2)
        {
            return Topla(ədəd2, ədəd1);
        }
        /// <summary>
        /// BigInteger tipli və HyperFloat tipli ədədləri toplayır
        /// </summary>
        /// <param name="ədəd1">Birinci ədəd</param>
        /// <param name="ədəd2">İkinci ədəd</param>
        /// <returns></returns>
        public static HyperFloat Topla(BigInteger ədəd1, HyperFloat ədəd2)
        {
            return Topla(ədəd2, ədəd1);
        }
        #endregion

        //Bura qədər sənədləşib. namaz qılıb qayıdıram :D
        #region Çıxma və onun override metodları
        public static HyperFloat Cix(HyperFloat eded1, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            //Console.WriteLine("eded1=" + eded1);
            //Console.WriteLine("eded2=" + eded2);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(HyperFloat eded1, int eded2_)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded2 = new HyperFloat(eded2_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(HyperFloat eded1, string eded2_)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded2 = new HyperFloat(eded2_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(HyperFloat eded1, long eded2_)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded2 = new HyperFloat(eded2_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(HyperFloat eded1, ulong eded2_)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded2 = new HyperFloat(eded2_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(HyperFloat eded1, double eded2_)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded2 = new HyperFloat(eded2_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(HyperFloat eded1, BigInteger eded2_)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded2 = new HyperFloat(eded2_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(int eded1_, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded1 = new HyperFloat(eded1_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(string eded1_, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded1 = new HyperFloat(eded1_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(long eded1_, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded1 = new HyperFloat(eded1_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(ulong eded1_, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded1 = new HyperFloat(eded1_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(double eded1_, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded1 = new HyperFloat(eded1_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Cix(BigInteger eded1_, HyperFloat eded2)
        {
            //   a/b+c/d=(ad+bc)/bd
            HyperFloat eded1 = new HyperFloat(eded1_);
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d - b * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        #endregion

        #region Hasil və onun override metodları
        public static HyperFloat Hasil(HyperFloat eded1, HyperFloat eded2)
        {
            //   a/b * c/d=ac/bd
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * c;
            var cvb_m = b * d;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Hasil(HyperFloat eded1, int eded2)
        {
            //   a/b * c/d=ac/bd
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a * eded2;
            var cvb_m = b;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Hasil(HyperFloat eded1, long eded2)
        {
            //   a/b * c/d=ac/bd
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a * eded2;
            var cvb_m = b;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Hasil(HyperFloat eded1, ulong eded2)
        {
            //   a/b * c/d=ac/bd
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a * eded2;
            var cvb_m = b;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Hasil(HyperFloat eded1, string eded2)
        {
            return Hasil(eded1, new HyperFloat(eded2));
        }
        public static HyperFloat Hasil(HyperFloat eded1, double eded2)
        {
            return Hasil(eded1, new HyperFloat(eded2));
        }
        public static HyperFloat Hasil(HyperFloat eded1, BigInteger eded2)
        {
            //   a/b * c/d=ac/bd
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a * eded2;
            var cvb_m = b;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Hasil(int eded1, HyperFloat eded2)
        {
            return Hasil(eded2, eded1);
        }
        public static HyperFloat Hasil(long eded1, HyperFloat eded2)
        {
            return Hasil(eded2, eded1);
        }
        public static HyperFloat Hasil(ulong eded1, HyperFloat eded2)
        {
            return Hasil(eded2, eded1);
        }
        public static HyperFloat Hasil(string eded1, HyperFloat eded2)
        {
            return Hasil(eded2, eded1);
        }
        public static HyperFloat Hasil(double eded1, HyperFloat eded2)
        {
            return Hasil(eded2, eded1);
        }
        public static HyperFloat Hasil(BigInteger eded1, HyperFloat eded2)
        {
            return Hasil(eded2, eded1);
        }
        #endregion

        #region Bolme ve onun override metodları
        public static HyperFloat Bolme(HyperFloat eded1, HyperFloat eded2)
        {
            //   a/b / c/d=ad/bc
            var a = eded1.suret;
            var c = eded2.suret;
            var b = eded1.mexrec;
            var d = eded2.mexrec;
            var cvb_s = a * d;
            var cvb_m = b * c;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(HyperFloat eded1, int eded2)
        {
            //   a/b / c/d=ad/bc
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a;
            var cvb_m = b * eded2;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(HyperFloat eded1, long eded2)
        {
            //   a/b / c/d=ad/bc
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a;
            var cvb_m = b * eded2;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(HyperFloat eded1, ulong eded2)
        {
            //   a/b / c/d=ad/bc
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a;
            var cvb_m = b * eded2;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(HyperFloat eded1, string eded2)
        {
            return Bolme(eded1, new HyperFloat(eded2));
        }
        public static HyperFloat Bolme(HyperFloat eded1, double eded2)
        {
            return Bolme(eded1, new HyperFloat(eded2));
        }
        public static HyperFloat Bolme(HyperFloat eded1, BigInteger eded2)
        {
            //   a/b / c/d=ad/bc
            var a = eded1.suret;
            var b = eded1.mexrec;
            var cvb_s = a;
            var cvb_m = b * eded2;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(int eded1, HyperFloat eded2)
        {
            //   a : b/c=ac/b
            var cvb_s = eded1 * eded2.mexrec;
            var cvb_m = eded2.suret;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(long eded1, HyperFloat eded2)
        {
            //   a : b/c=ac/b
            var cvb_s = eded1 * eded2.mexrec;
            var cvb_m = eded2.suret;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(ulong eded1, HyperFloat eded2)
        {
            //   a : b/c=ac/b
            var cvb_s = eded1 * eded2.mexrec;
            var cvb_m = eded2.suret;
            return new HyperFloat(cvb_s, cvb_m);
        }
        public static HyperFloat Bolme(string eded1, HyperFloat eded2)
        {
            return Bolme(new HyperFloat(eded1), eded2);
        }
        public static HyperFloat Bolme(double eded1, HyperFloat eded2)
        {
            return Bolme(new HyperFloat(eded1), eded2);
        }
        public static HyperFloat Bolme(BigInteger eded1, HyperFloat eded2)
        {
            //   a : b/c=ac/b
            var cvb_s = eded1 * eded2.mexrec;
            var cvb_m = eded2.suret;
            return new HyperFloat(cvb_s, cvb_m);
        }
        #endregion

        #region toplama operatorlari
        public static HyperFloat operator +(HyperFloat eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(HyperFloat eded1, int eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(HyperFloat eded1, long eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(HyperFloat eded1, string eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(HyperFloat eded1, ulong eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(HyperFloat eded1, double eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(HyperFloat eded1, BigInteger eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(int eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(long eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(ulong eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(double eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(string eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        public static HyperFloat operator +(BigInteger eded1, HyperFloat eded2)
        {
            return Topla(eded1, eded2);
        }
        #endregion

        #region Cixma operatorlari
        public static HyperFloat operator -(HyperFloat eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(HyperFloat eded1, int eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(HyperFloat eded1, long eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(HyperFloat eded1, string eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(HyperFloat eded1, ulong eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(HyperFloat eded1, double eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(HyperFloat eded1, BigInteger eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(int eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(long eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(ulong eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(double eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(string eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        public static HyperFloat operator -(BigInteger eded1, HyperFloat eded2)
        {
            return Cix(eded1, eded2);
        }
        #endregion

        #region Vurma operatorlari
        public static HyperFloat operator *(HyperFloat eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(HyperFloat eded1, int eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(HyperFloat eded1, long eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(HyperFloat eded1, string eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(HyperFloat eded1, ulong eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(HyperFloat eded1, double eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(HyperFloat eded1, BigInteger eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(int eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(long eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(ulong eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(double eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(string eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        public static HyperFloat operator *(BigInteger eded1, HyperFloat eded2)
        {
            return Hasil(eded1, eded2);
        }
        #endregion

        #region Bolme operatorlari
        public static HyperFloat operator /(HyperFloat eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(HyperFloat eded1, int eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(HyperFloat eded1, long eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(HyperFloat eded1, string eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(HyperFloat eded1, ulong eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(HyperFloat eded1, double eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(HyperFloat eded1, BigInteger eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(int eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(long eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(ulong eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(double eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(string eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        public static HyperFloat operator /(BigInteger eded1, HyperFloat eded2)
        {
            return Bolme(eded1, eded2);
        }
        #endregion

        #region elave operatorlar
        public static HyperFloat operator -(HyperFloat eded)
        {
            return new HyperFloat(eded.suret * (-1), eded.mexrec);
        }
        public static HyperFloat operator --(HyperFloat eded)
        {
            return new HyperFloat(eded.suret - eded.mexrec, eded.mexrec);
        }
        public static HyperFloat operator ++(HyperFloat eded)
        {
            return new HyperFloat(eded.suret + eded.mexrec, eded.mexrec);
        }
        #endregion

        #region boyuk kicik operatorlari
        public static bool operator >(HyperFloat eded1, HyperFloat eded2)
        {
            var t = Boyukdur(eded1, eded2);
            if (t == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator <(HyperFloat eded1, HyperFloat eded2)
        {
            var t = Boyukdur(eded2, eded1);

            if (t == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator >=(HyperFloat eded1, HyperFloat eded2)
        {
            var t = Boyukdur(eded1, eded2);
            if (t == 1 || t == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator <=(HyperFloat eded1, HyperFloat eded2)
        {
            var t = Boyukdur(eded2, eded1);

            if (t == 1 || t == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Basqa tipden bu tipe cevrilme operatorlari
        public static implicit operator HyperFloat(int eded)
        {
            return new HyperFloat(eded);
        }
        public static implicit operator HyperFloat(long eded)
        {
            return new HyperFloat(eded);
        }
        public static implicit operator HyperFloat(ulong eded)
        {
            return new HyperFloat(eded);
        }
        public static implicit operator HyperFloat(double eded)
        {
            return new HyperFloat(eded);
        }
        public static implicit operator HyperFloat(string eded)
        {
            return new HyperFloat(eded);
        }
        public static implicit operator HyperFloat(BigInteger eded)
        {
            return new HyperFloat(eded);
        }
        #endregion

        #region Bu tipdən başqa tiplərə çevirmə metodları
        public BigInteger to_BigInteger()
        {
            BigInteger t = this.suret / this.mexrec;
            return t;
        }
        public int to_int()
        {
            var t = (this.suret / this.mexrec);
            if (t > int.MaxValue)
            {
                throw new Exception("Cox boyuk eded ");
            }
            else
            {
                return (int)Convert.ToDouble(t.ToString());
            }
        }
        public long to_long()
        {
            var t = (this.suret / this.mexrec);
            if (t > long.MaxValue)
            {
                throw new Exception("Cox boyuk eded ");
            }
            else
            {
                return (long)Convert.ToDouble(t.ToString());
            }
        }
        public ulong to_ulong()
        {
            var t = (this.suret / this.mexrec);
            if (t > ulong.MaxValue)
            {
                throw new Exception("Cox boyuk eded ");
            }
            else
            {
                return (ulong)Convert.ToDouble(t.ToString());
            }
        }
        public double to_double()
        {
            return Convert.ToDouble(this.To_Double_str(deqiqlik));
        }
        public string To_Double_str(int n)
        {
            string s = "";
            var sur = suret;
            var mex = mexrec;
            BigInteger tam = sur / mex;
            s += tam + ".";
            sur = sur - mex * tam;
            sur = BigInteger.Abs(sur);
            for (int i = 0; i < n; i++)
            {
                var t = (10 * sur) / mex;
                s += t;
                sur = (10 * sur - t * mex);
            }
            return s;
        }

        #endregion

        #region ozunun metodlari
        public bool SAME(HyperFloat eded)
        {
            if (eded.suret == this.suret)
            {
                if (eded.mexrec == this.mexrec)
                {
                    return true;
                }
            }
            return false;
        }
        private void Yoxla()
        {
            if (deqiqlik < 0)
            {
                throw new Exception("Deqiqlik menfi ola bilmez");
            }
            var t = HyperFloat.EBOB(this.suret, this.mexrec);
            if (t.ToString() != "0" && t.ToString() != "1")
            {
                //Console.WriteLine("ixtisardan evvel");
                //Console.WriteLine(this.suret + " - " + this.mexrec);
                //Console.WriteLine("\t\t"+suret + " / " + t + " = " + suret / t);
                suret /= t;
                mexrec /= t;
                //Console.WriteLine("\n\nixtisar etdim");
                //Console.WriteLine(this.suret + " - " + this.mexrec);
            }
            //if(suret.ToString().Length>deqiqlik) suret = BigInteger.Parse(suret.ToString().Substring(0, deqiqlik));
            //if (mexrec.ToString().Length > deqiqlik) mexrec = BigInteger.Parse(mexrec.ToString().Substring(0, deqiqlik));
            var cari = this.To_Double_str(deqiqlik).ToString();
            cari = son_sifirlari_sil(cari);
            //Console.WriteLine("habda cari=" + cari);
            if (cari.Length > deqiqlik)
            {
                //Console.WriteLine("formatlanir..." + cari);
                var yeni = (cari.Substring(0, deqiqlik));
                this.suret = get_suret_from_eded(yeni);
                this.mexrec = get_mexrec_from_eded(yeni);
                //Console.WriteLine("Indi bax buna");
                //Console.WriteLine(this.suret + " / " + this.mexrec);
                //Console.WriteLine("yeni forma..." + yeni);
                //Console.ReadKey();
            }
        }
        #endregion

        #region override metodlari
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, null))
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            HyperFloat vari = null;

            try
            {
                vari = new HyperFloat(obj.ToString());
            }
            catch (Exception)
            {
                return false;
            }

            if (vari is HyperFloat)
            {
                //Console.WriteLine("Eyniliyi yoxlanilir..."+obj.ToString());
                return vari.SAME(this);
            }
            else
            {
                //Console.WriteLine("HAI?");
                return base.Equals(obj);
            }
        }
        public override string ToString()
        {
            // return "(" + suret + " ; " + mexrec + ")";
            return son_sifirlari_sil(this.To_Double_str(deqiqlik));
        }
        public override int GetHashCode()
        {
            return this.suret.GetHashCode() ^ this.mexrec.GetHashCode();
        }
        #endregion

        #region static metodlar
        public static int Boyukdur(HyperFloat eded1, HyperFloat eded2)
        {
            if (eded1.İşarə > eded2.İşarə)
            {
                return 1;
            }
            if (eded1.SAME(eded2))
            {
                return 0;
            }
            var x1 = eded1.suret * eded2.mexrec;
            var x2 = eded2.suret * eded1.mexrec;

            if (x1 > x2)
            {
                return 1;
            }
            else
            {
                return -1;
            }

        }
        public static HyperFloat SQRT(HyperFloat eded)
        {
            if (eded.suret < 0)
            {
                throw new Exception("Menfi ededden kok alinmaz");
            }
            if (eded.suret == 0)
            {
                return 0;
            }
            HyperFloat cvb = 1;
            var cache = new HyperFloat(-1);
            int say = 0;
            while (!cvb.SAME(cache))
            {
                cache = cvb;
                cvb = (cvb + eded / cvb) / 2.0;
                // Console.WriteLine("araliq:  " + cvb.ToString());
                say++;
                if (say > 100)
                {
                    return cvb;
                }
            }
            return cvb;
        }
        public static HyperFloat SQR(HyperFloat eded)
        {
            return eded * eded;
        }
        public static HyperFloat POWER(HyperFloat esas, BigInteger quvvet)
        {
            //            Console.WriteLine("powering...      " + esas.ToString() + " ^ " + quvvet.ToString());
            if (quvvet == 0)
            {
                if (esas.ToString() != "0.0")
                {
                    return new HyperFloat(1);
                }
                else
                {
                    throw new Exception("Xeta");
                }
            }
            if (quvvet < 0)
            {
                //Console.WriteLine("zad");
                return new HyperFloat("1") / POWER(esas, -quvvet);
            }
            if (quvvet % 2 == 0)
            {
                var t = POWER(esas, quvvet / 2);
                return t * t;
            }
            else
            {
                //Console.WriteLine("Xiyar");
                //Console.ReadKey();
                var t = POWER(esas, (quvvet - 1) / 2);
                return new HyperFloat((t * t * esas).ToString());
            }
        }
        public static HyperFloat ABS(HyperFloat x)
        {
            if (x.suret >= 0)
            {
                return x;
            }
            else
            {
                return -x;
            }
        }
        public static HyperFloat EXP_KESR(HyperFloat eded)
        {
            int i = 1;

            HyperFloat cvb = "1";
            HyperFloat vuruq = "1";
            HyperFloat cache = "-1";
            HyperFloat ferq = HyperFloat.ABS(cache - cvb);
            while (ferq.ToString() != "0.0")
            {
                // Console.WriteLine("cvb-cache = "+cvb.ToString() + " - " + cache.ToString());
                cache = cvb.ToString();
                vuruq *= (eded / new HyperFloat(i.ToString()));
                cvb = cvb + vuruq;
                i++;
                ferq = HyperFloat.ABS(cache - cvb);
                //Console.WriteLine("ferq{" + i + "}=" + ferq.ToString());
            }
            //Console.WriteLine("yekun:   "+cvb.ToString()); 
            return new HyperFloat(cvb.ToString());

        }
        public static HyperFloat EXP_TAM(HyperFloat eded)
        {
            BigInteger tam = eded.to_BigInteger();
            return POWER(E, tam);
        }
        public static HyperFloat EXP(HyperFloat eded)
        {
            BigInteger tam = eded.to_BigInteger();
            HyperFloat tam_quv = EXP_TAM(eded);
            HyperFloat kesr = eded - new HyperFloat(tam);
            var kesr_quv = EXP_KESR(kesr);
            return kesr_quv * tam_quv;
        }
        public static HyperFloat LN(HyperFloat eded, int xeta)
        {
            if (eded <= 0)
            {
                throw new Exception("Menfi ededin loqarifmasi olmaz");
            }
            if (eded.ToString() == "1.0")
            {
                return 0;
            }
            //Console.WriteLine("lnning   " + eded.ToString());
            HyperFloat x = 1;
            HyperFloat cache = -1;
            HyperFloat ferq = cache - x;
            //Console.WriteLine("ferq=    " + ferq.ToString());
            while (ferq.ToString().Substring(0, ferq.ToString().Length - xeta).Replace("0", "") != ".")
            {
                cache = x;
                x = x - 1 + eded.ToString() / HyperFloat.EXP(x);
                //Console.WriteLine((cache - x).ToString());
                // Console.ReadKey();
                //Console.WriteLine("x,cache =" + x.ToString() + " - " + cache.ToString());
                ferq = cache - x;
                // Console.WriteLine("ferq=    " + ferq.ToString());
            }
            //Console.WriteLine("cavab:\n\n\n");
            return x;
        }
        public static BigInteger EBOB(BigInteger a_, BigInteger b_)
        {
            BigInteger a = a_;
            BigInteger b = b_;
            //Console.Write("EBOB("+a + " ; " + b+" ) =");
            a = BigInteger.Abs(a);
            b = BigInteger.Abs(b);
            if (a == 1 || b == 1)
            {
                return 1;
            }
            if (a == 0 || b == 0)
            {
                return 0;
            }

            while (a != b)
            {
                //Console.WriteLine("zad="+a + " * " + b);

                if (a > b)
                {
                    a %= b;
                }
                else
                    if (b > a)
                    {
                        b %= a;
                    }
                if (a == 0)
                {
                    return b;
                }
                if (b == 0)
                {
                    return a;
                }
            }
            //Console.WriteLine(a);
            return a;
        }
        static string son_sifirlari_sil(string eded)
        {
            string s = "";
            bool cix = false;

            for (int i = eded.Length - 1; i > 0; i--)
            {
                // Console.WriteLine(i);
                if (eded[i] != '0')
                {
                    cix = true;
                    var cvb = eded.Substring(0, i + 1);
                    if (cvb[cvb.Length - 1] == '.')
                    {
                        return cvb + "0";
                    }
                    else
                    {
                        return cvb;
                    }
                }
                if (cix)
                {
                    break;
                }
            }
            throw new Exception("Burda diyirdma");
            return "A";
        }
        /// <summary>
        /// Sətir olaraq verilmiş ədədi adi kəsr şəklinə çevirir və həmin kəsrin surətini qaytarır
        /// </summary>
        /// <param name="eded"></param>
        /// <returns></returns>
        static BigInteger get_suret_from_eded(string eded)
        {
            int isare = 1;
            if (eded[0] == '-')
            {
                isare = -1;
                eded = eded.Substring(1, eded.Length - 1);
            }
            if (eded.Contains('.'))
            {
                int uz = eded.ToString().Length;
                int noq = eded.IndexOf('.');
                int ferq = uz - noq - 1;
                var zad = eded.Replace(".", "");
                return isare * BigInteger.Parse(zad);
            }
            else
            {
                return isare * BigInteger.Parse(eded);
            }
        }
        /// <summary>
        /// Sətir olaraq verilmiş ədədi adi kəsr şəklinə çevirir və həmin kəsrin məxrəcini qaytarır
        /// </summary>
        /// <param name="eded"></param>
        /// <returns></returns>
        static BigInteger get_mexrec_from_eded(string eded)
        {
            if (eded[0] == '-')
            {
                eded = eded.Substring(1, eded.Length - 1);
            }
            if (eded.Contains('.'))
            {
                int uz = eded.ToString().Length;
                int noq = eded.IndexOf('.');
                int ferq = uz - noq - 1;
                return BigInteger.Parse("1" + new string('0', ferq));
            }
            else
            {
                return 1;
            }
        }
        #endregion
    }
}

/*Gələcək metodlar
 * sin
 * cos
 * tan
 * ctan
 * sh
 * ch
 * faktorial
 * qamma
 * əlavə imkanlar
 * xüsusi dəqiqlikdə
 * 
 * inteqral
 * limit
 * diferensial tənlik
 * f(x)=0
 * 
 */
