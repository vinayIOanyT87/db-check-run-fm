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
    public class DownloadReferenceDataRequest : ExchangeRequestBase
    {
		/// <summary>
		/// Gets or sets a value indicating whether to generate file only.
		/// </summary>
		/// <value>
		///   <c>true</c> if [generate file only]; otherwise, <c>false</c>.
		/// </value>
		public bool GenerateFileOnly { get; set; }
		
		public bool UsersOnly = false;

	    public string EntityNames { get; set; }
		
		public DownloadReferenceDataRequest()
        {
            this.ExchangeType = ExchangeType.DownloadReferenceData;
        }        
    }
}
