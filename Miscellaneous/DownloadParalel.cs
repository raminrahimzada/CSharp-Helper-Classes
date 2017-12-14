using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace get_gelirem_oyunu
{
    class DownloadParalel
    {
        public static void ParallelDownloadFile(string _uri_, string _filePath_, int _chunkSize_,int _size_)
        {
            if (_uri_ == null)
                throw new ArgumentNullException("_uri_");

            // determine file size first
            //long size = GetFileSize(uri);
            using (var file = new FileStream(_filePath_, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                file.SetLength(_size_); // set the length first

                var syncObject = new object(); // synchronize file writes
                Parallel.ForEach(LongRange(0, 1 + _size_ / _chunkSize_), (_start_) =>
                {
                    var request = (HttpWebRequest)WebRequest.Create(_uri_);
                    request.AddRange(_start_ * _chunkSize_, _start_ * _chunkSize_ + _chunkSize_ - 1);
                    var response = (HttpWebResponse)request.GetResponse();

                    lock (syncObject)
                    {
                        Console.WriteLine("fayla yaziram:" + _start_);
                        using (var stream = response.GetResponseStream())
                        {
                            file.Seek(_start_ * _chunkSize_, SeekOrigin.Begin);
                            stream.CopyTo(file);
                        }
                    }
                });
            }
        }

        public static long GetFileSize(string uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "HEAD";
            var response = (HttpWebResponse)request.GetResponse();
            return response.ContentLength;
        }

        private static IEnumerable<long> LongRange(long start, long count)
        {
            long i = 0;
            while (true)
            {
                if (i >= count)
                {
                    yield break;
                }
                yield return start + i;
                i++;
            }
        }
    }
}
