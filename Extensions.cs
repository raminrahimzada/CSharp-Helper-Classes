
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace raminrahimzada
{
    public static class Extensions
    {
        public static string DataTableToJSON(this System.Data.DataTable dt)
        {
            var serializer = new JavaScriptSerializer();
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
         public static string Ci(this int eded)
        {
            var s = eded + "";
            s = s.Trim();
            var sLast = s.Last();
            switch (sLast)
            {
                case '1':
                case '2':
                case '5':
                case '7':
                case '8':
                    return s + "-ci";
                case '3':
                case '4':
                    return s + "-cü";
                case '9':
                    return s + "-cu";
                case '6':
                    return s + "-cı";
            }
            if (sLast != '0') throw new NotImplementedException("Bura duşməli deyildin axi :(");

            if (s.Length < 2) return s + "cı";

            switch (s[s.Length - 2])
            {
                case '1':
                case '3':
                    return s + "-cu";
                case '2':
                case '5':
                case '7':
                case '8':
                    return s + "-ci";
                case '4':
                case '6':
                case '9':
                    return s + "-cı";
            }
            if (s[s.Length - 2] != '0') throw new NotImplementedException("Bura duşməli deyildin axi :(");

            if (s.EndsWith("2000"))
                return s + "ci";
            return s + "cü";
        }

        public static string SafeXSS(this string html)
        {
            return Regex.Replace(
                html,
                @"</?(?i:script|embed|object|frameset|frame|iframe|meta|link|style)(.|\n|\s)*?>",
                string.Empty,
                RegexOptions.Singleline|RegexOptions.IgnoreCase
            );
        }
        public static string DownloadHTML(this string url)
        {
            using (var cl=new WebClient())
            {
                cl.Encoding=Encoding.UTF8;
                var data = cl.DownloadData(url);
                var html = Encoding.UTF8.GetString(data);
                return html;
            }
        }
        public static IEnumerable<T> ExceptLast<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var it = source.GetEnumerator();
            bool hasRemainingItems;
            bool isFirst = true;
            T item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
            it.Dispose();
        }
        public static int ToInt(this string s, int eded)
        {
            int result;
            return int.TryParse(s, out result) ? result : eded;
        }
        public static int ToInt(this string s )
        {
            return int.Parse(s);
        }
        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string DateTime2Date(this object obj)
        {
            DateTime dt;
            try
            {
                dt = DateTime.Parse(obj.ToString());
            }
            catch
            {
                return "-";
            }
            if (dt.Equals(DateTime.MaxValue))
            {
                return "-";
            }
            return dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year;
        }

        public static string ToSqlString(this DateTime dt)
        {
            return string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute,
                dt.Second,
                dt.Millisecond);
        }

        public static DateTime TryToParseDateTime(this object obj)
        {
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            return DateTime.Parse(obj.ToString());
        }
        public static DateTime TryToParseDateTime(this object obj, DateTime defaulTime)
        {
            if (obj == null)
            {
                return defaulTime;
            }
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return defaulTime;
            }
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            try
            {
                return DateTime.Parse(obj.ToString());
            }
            catch
            {
                return defaulTime;
            }
        }

        public static bool IsDateTime(this object obj, out DateTime? dt)
        {
            dt = null;
            if (obj == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return false;
            }
            if (obj is DateTime)
            {
                dt = (DateTime) obj;
                return true;
            }
            try
            {
                dt = DateTime.Parse(obj.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsDateTime(this object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return false;
            }
            if (obj is DateTime)
            {
                return true;
            }
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                DateTime.Parse(obj.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static DateTime DateTimeParse(this string str)
        {
            var hamisi = str.Split(':', '/', '-', '.');

            var day = hamisi[0].ToInt();
            var mon = hamisi[1].ToInt();
            var year = hamisi[2].ToInt();

            if (mon > 12 || mon < 1)
            {
                throw new Exception();
            }
            if (day > 31 || day < 1)
            {
                throw new Exception();
            }
            if (year > DateTime.Now.Year || year < 1900)
            {
                throw new Exception();
            }
            return new DateTime(year, mon, day);
        }
        public static void SendOnlyThis(this HttpResponse response, string data)
        {
            response.Clear();
            response.ClearContent();
            response.Write(data);
            response.End();
        }
        public static string FirstToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            string cavab;
            if (s.Contains(" ")) cavab= string.Join(" ", s.Split(' ').Select(ss => ss.FirstToUpper()));
            else
            if (string.IsNullOrEmpty(s))
            {
                cavab= s;
            }else
            if (s.Length == 1)
            {
                cavab=s.ToUpper();
            }else
                cavab = s[0].ToString().ToUpper() + s.Substring(1).ToLower(new CultureInfo("az-Latn-AZ"));
            return cavab;
        }

        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

       
    }
}
