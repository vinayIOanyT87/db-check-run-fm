// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NspaExchangeRequest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the NspaExchangeRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
    using System.Collections.Generic;

    using global::Nspa;

	public class UploadTransactionsRequest : ExchangeRequestBase
    {
        public UploadTransactionsRequest()
        {
            this.ExchangeType = ExchangeType.UploadTransactions;
			this.TransactionList = new List<AdcTransactionDoGenerated>();
        }

		public List<AdcTransactionDoGenerated> TransactionList { get; set; }
    }
}
