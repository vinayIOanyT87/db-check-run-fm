// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NspaExchangeRequest.cs" company="Varec, Inc.">
//   
// </copyright>
// <summary>
//   Defines the NspaExchangeRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class UploadLogFileRequest : ExchangeRequestBase
    {
        public UploadLogFileRequest()
        {
            this.ExchangeType = ExchangeType.UploadLogFile;
            this.CompressedLogData = null;
        }

        public string LogFileName { get; set; }

        public byte[] CompressedLogData;

        public void SetLogData(string logText)
        {

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(logText)))
            {
                CompressedLogData = Compress(stream);
            }

            // Decompress compressed bytes
            using (var decompressedStream = Decompress(CompressedLogData))
            using (var reader = new StreamReader(decompressedStream))
            {
                var decompressedValue = reader.ReadToEnd();

                if (logText == decompressedValue)
                    Console.WriteLine("Success");
                else
                    Console.WriteLine("Failed");
            }
        }

        public static byte[] Compress(Stream input)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                CopyStream(input, zipStream);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static Stream Decompress(byte[] data)
        {
            var output = new MemoryStream();
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                CopyStream(zipStream, output);
                zipStream.Close();
                output.Position = 0;
                return output;
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        public static UploadLogFileRequest CreateRequestFromXml(string xmlData)
        {
            var stringReader = new StringReader(xmlData);
            var xtr = new XmlTextReader(stringReader);
			var xmlSerializer = new XmlSerializer(typeof(UploadLogFileRequest), string.Empty);
            var request = (UploadLogFileRequest)xmlSerializer.Deserialize(xtr);
            return request;
        }
    }
}
