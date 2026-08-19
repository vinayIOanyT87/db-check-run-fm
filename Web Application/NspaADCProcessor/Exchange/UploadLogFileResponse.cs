// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadTransactionsResponse.cs" company="Varec, Inc.">
//   
// </copyright>
// <summary>
//   Defines the DownloadTransactionsResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
    public class UploadLogFileResponse : ExchangeResponseBase
    {
        public UploadLogFileResponse()
        {
            this.ExchangeType = ExchangeType.UploadLogFile;
        }
    }
}
