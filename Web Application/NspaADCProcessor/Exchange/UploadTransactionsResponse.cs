// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTransactionsResponse.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UploadTransactionsResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
    using System.Collections.Generic;

    public class UploadTransactionsResponse : ExchangeResponseBase
    {
        public UploadTransactionsResponse()
        {
            this.ExchangeType = ExchangeType.UploadTransactions;
            this.TransactionStatusList = new List<UploadTransactionResponse>();
        }

        public List<UploadTransactionResponse> TransactionStatusList { get; set; }
    }
}
