using System;
using System.IO;
using System.IO.Compression;
using System.Web.UI;

public class CompressedWebPage :  Page
    {
        private readonly ObjectStateFormatter _formatter = new ObjectStateFormatter();
        public  const string CompressedViewstateKey = "__COMPRESSED_VIEWSTATE";
        private static byte[] Compress(byte[] data)
        {
            var compressedData = new MemoryStream();
            var compressStream = new GZipStream(compressedData, CompressionMode.Compress, true);
            compressStream.Write(data, 0, data.Length);
            compressStream.Close();
            return compressedData.ToArray();
        }

        private static byte[] Uncompress(byte[] data)
        {
            var compressedData = new MemoryStream();
            compressedData.Write(data, 0, data.Length);
            compressedData.Position = 0;
            var compressStream = new GZipStream(compressedData, CompressionMode.Decompress, true);
            var uncompressedData = new MemoryStream();
            var buffer = new byte[64];
            var read = compressStream.Read(buffer, 0, buffer.Length);

            while (read > 0)
            {
                uncompressedData.Write(buffer, 0, read);
                read = compressStream.Read(buffer, 0, buffer.Length);
            }
            compressStream.Close();
            return uncompressedData.ToArray();
        }

        protected override void SavePageStateToPersistenceMedium(object viewState)
        {
            var ms = new MemoryStream();
            _formatter.Serialize(ms, viewState);
            var viewStateBytes = ms.ToArray();
            ClientScript.RegisterHiddenField(CompressedViewstateKey
                , Convert.ToBase64String(Compress(viewStateBytes)));
        }

        protected override object LoadPageStateFromPersistenceMedium()
        {
            var compressedViewState = Request.Form[CompressedViewstateKey];
            var bytes = Uncompress(Convert.FromBase64String(compressedViewState));
            return _formatter.Deserialize(Convert.ToBase64String(bytes));
        }
    }
