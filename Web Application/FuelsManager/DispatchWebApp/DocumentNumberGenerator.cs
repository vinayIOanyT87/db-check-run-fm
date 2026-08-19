namespace FuelsManager.DispatchWebApp
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class DocumentNumberGenerator
	{
		private readonly SecurityClass security;

		/// <summary>
		/// This is the default constructor for the Generate Document Numbers class.
		/// </summary>
		/// <param name="inSecurity">The security object.</param>
		public DocumentNumberGenerator(SecurityClass inSecurity)
		{
			this.security = inSecurity;
		}

		/// <summary>
		/// This method will return a new document number from tblSites table based
		/// on the transaction type ID.
		/// </summary>
		/// <param name="transTypeId">Transaction Type</param>
		/// <returns>Returns a document number in string form.</returns>
		public string GetNextDocumentNumber(TransactionTypes transTypeId)
		{
			if (transTypeId == TransactionTypes.T5_PrimaryDisbursement
				|| transTypeId == TransactionTypes.T25_Shipment)
			{
				return FMChannelHelper.MakeCall<ISites, string>(
						x => x.GetNextDocumentNumber(this.security, DOCUMENT_TYPE.MANUAL_BOL, this.security.SiteGuid));

			}

			if ((transTypeId == TransactionTypes.T17_Order)
				|| (transTypeId == TransactionTypes.T18_SupplyOrder))
			{
				return FMChannelHelper.MakeCall<ISites, string>(
					x => x.GetNextDocumentNumber(this.security, DOCUMENT_TYPE.ORDER, this.security.SiteGuid));

			}

			return FMChannelHelper.MakeCall<ISites, string>(
				x => x.GetNextDocumentNumber(this.security, DOCUMENT_TYPE.TRANSACTION, this.security.SiteGuid));
		}
	}
}