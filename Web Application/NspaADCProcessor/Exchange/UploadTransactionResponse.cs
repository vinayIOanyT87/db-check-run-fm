// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTransactionResponse.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UploadTransactionResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ADC.Nspa.General
{
	public class UploadTransactionResponse : ExchangeResponseBase
    {
        public string TransactionId { get; set; }

		public UploadTransactionResponse()
		{
            this.ExchangeType = ExchangeType.UploadTransactions;
        }
    }
}