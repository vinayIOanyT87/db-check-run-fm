// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTransactionsRequestProcessor.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UploadTransactionsRequestProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nspa
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;

    using ADC.Nspa.General;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

	public class UploadLogFileRequestProcessor : RequestProcessorGenericBase<UploadLogFileRequest, UploadLogFileResponse>
    {
		internal UploadLogFileRequestProcessor()
			: base("uploading a log file")
		{
			
		}

		protected override void ProcessCore()
		{

			ValidateExchangeUserId(this.Security.UserID);
            this.Response.Success = false;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(
					siteInterface => siteInterface.GetByID(this.Security, this.Security.SiteID, false));

            if (!site.SiteGuid.IsEmpty())
            {
                var directoryName = site.ImportArchiveDir;
	            if (string.IsNullOrEmpty(directoryName))
                {
                    string errorMessage = string.Format("Site {0} does not have a properly configured Data Transmission Import Archive Directory.", this.Security.SiteID);
                    AddResponseError("Upload Log File", errorMessage);
                }
                else
                {
	                string decompressedValue;
	                using (var decompressedStream = Decompress(this.Request.CompressedLogData))
                    using (var reader = new StreamReader(decompressedStream))
                    {
                        decompressedValue = reader.ReadToEnd();
                    }

                    var fileName = Path.Combine(directoryName, this.Request.LogFileName);
                    File.WriteAllText(fileName, decompressedValue);
                    this.Response.Success = true;
                }
            }
           
        }

        #region #helper functions
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
        #endregion
    }
}
