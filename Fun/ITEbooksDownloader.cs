using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Matrix_Recognition
{
    class ITEbooksDownloader
    {
        public const int LEN = 1024*1024;
        private string _location;
        [STAThread]
        public   void Downlaod(int _bookId_,string _location_)
        {
            Sonlandi = false;
            _ok = false;
            BookId = _bookId_;
            _location = _location_;
            _ok = false;
            var url = string.Format("http://it-ebooks.info/book/{0}/", _bookId_);
            var browser = new WebBrowser();
            browser.Navigate(url);
            browser.DocumentCompleted += browser_DocumentCompleted;
            while (Sonlandi)
            {
                Application.DoEvents();
            }
        }

        bool _ok;
        public bool Sonlandi { get; private set; }
        private   void browser_DocumentCompleted(object _sender_, WebBrowserDocumentCompletedEventArgs _e_)
        {
            if (_ok) return;
            var brow = (_sender_ as WebBrowser);
            if (brow == null) return;
            if (brow.Document == null) return;
            if (brow.Document.Body == null) return;
            var tttt = brow.Document.Body.GetElementsByTagName("A")
                .Cast<HtmlElement>()
                .Select(_a_ => _a_.GetAttribute("href").ToLower())
                .Where(_href_ => _href_.Contains("filepi"))
                .ToList();
            if (tttt.Count <= 0) return;
            _ok = true;
            var filePiUrl = tttt.First();
            var cookie = brow.Document.Cookie;
            var url = filePiUrl;
            var webRequest = (HttpWebRequest) WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.SetCookies(new Uri(url), cookie);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Host = "filepi.com";
            webRequest.Referer = "http://it-ebooks.info/book/4573/";
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var bufer = new byte[LEN];
            if (responseStream == null) return;
            var size = responseStream.Read(bufer, 0, bufer.Length);
            var fileStream = new FileStream(_location, FileMode.OpenOrCreate);
            Console.WriteLine("read : " + size);
            fileStream.Write(bufer, 0, size);
            while (size != 0)
            {
                size = responseStream.Read(bufer, 0, bufer.Length);
                fileStream.Write(bufer, 0, size);
                Console.WriteLine("read [{1}] : {0} bytes", size, BookId);
            }
            fileStream.Close();
            responseStream.Close();
            Console.WriteLine("read done.");
            Console.WriteLine("son");
            Sonlandi = true;
        }

        public int BookId { get; private set; }
    }

    class Test2
    {
        [STAThread]
        public static void Main(string[] _args_)
        {
            var dwnldr = new ItebooksDownloader();
            dwnldr.Downlaod(1024, "1024.pdf");
            while (!dwnldr.Sonlandi)
            {
                
            }
            Console.WriteLine("QUTARDI");
            Console.ReadKey();
        }

    }
}
