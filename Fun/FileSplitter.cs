using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSplitter
{
    class FileSplitter
    {
          static List<long> Parse(long _total_, long _max_)
        {
            var cvb = new List<long>();
            var qalan = _total_;
            while (_max_ < qalan)
            {
                cvb.Add(_max_);
                qalan -= _max_;
            }
            if (qalan != 0) cvb.Add(qalan);
            return cvb;
        }
        public static void Extract(string _fayl_, string _targetDir_, long _maxLengthInBytes_ = 300, long _buferLen_ = 100)
        {
            var faylLength = new FileInfo(_fayl_).Length;
            var bolguler = Parse(faylLength, _maxLengthInBytes_);
            var streamIn = File.Open(_fayl_, FileMode.Open);
            for (var current = 0; current < bolguler.Count; current++)
            {
                var bolgu = bolguler[current];
                var streamOut = File.OpenWrite(Path.Combine(_targetDir_, new FileInfo(_fayl_ + ".part" + current).Name));
                var qalan = bolgu;
                while (qalan > 0)
                {
                    int size;
                    if (qalan > _buferLen_)
                    {
                        var bufer = new byte[_buferLen_];
                        size = streamIn.Read(bufer, 0, bufer.Length);
                        streamOut.Write(bufer, 0, size);
                        qalan -= size;
                    }
                    else if (qalan != 0)
                    {
                        var bufer = new byte[qalan];
                        size = streamIn.Read(bufer, 0, bufer.Length);
                        streamOut.Write(bufer, 0, size);
                        qalan -= size;
                    }
                }
                streamOut.Close();
            }
            streamIn.Close();
        }
        public static void Join(string _esasfayl_, long _buferLen_ = 300, params string[] _fayllar_)
        {
            var streamOut = File.OpenWrite(_esasfayl_);
            foreach (var file in _fayllar_)
            {
                var streamIn = File.OpenRead(file);
                var size = -1;
                var bufer = new byte[_buferLen_];
                while (size != 0)
                {
                    size = streamIn.Read(bufer, 0, bufer.Length);
                    streamOut.Write(bufer, 0, size);
                }
                streamIn.Close();
            }
            streamOut.Close();
        }
    }
    class Program
    {
        public static void Main(string[] args)
        {
            const string fayl = @"C:\Users\Enver\Desktop\test.png";
            const string targetDir = @"C:\Users\Enver\Desktop\spl";
            const long maxLengthInBytes = 300; //1GB
            const int buferLen = 100;
            var fayllar = Enumerable.Range(0, 27).Select(_i_ => @"C:\Users\Enver\Desktop\spl\test.png.part" + _i_).ToArray();
            FileSplitter.Extract(fayl, targetDir, maxLengthInBytes);
            FileSplitter.Join(fayl + ".png", buferLen, fayllar);
        }
    }
}
