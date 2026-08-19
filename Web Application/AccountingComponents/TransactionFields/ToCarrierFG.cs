namespace TransactionFields
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ToCarrierFG.
	/// </summary>
	public class ToCarrierFG : CarrierFG
	{
		public ToCarrierFG()
		{
		}

		public override string FieldID { get { return "ToCarrierID"; } }
	
		public override object GetDataValue(TransactionDO transaction)
		{
			var ownerTransfer = transaction as OwnerTransferDO;

			if (ownerTransfer != null)
			{
				return ownerTransfer.ToCarrierID;
			}

			return string.Empty;
		}

		protected override CompanyCollectionClass GetEntries()
		{
			CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(transContext.security, COMPANY_ROLE.CARRIER,false,false)
																);

			return companyCollection;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			var ownerTransfer = transaction as OwnerTransferDO;

			if (ownerTransfer != null)
			{
				ownerTransfer.ToCarrierCode = newCode;
			}
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			var ownerTransfer = transaction as OwnerTransferDO;

			if (ownerTransfer != null)
			{
				ownerTransfer.ToCarrierID = newID;
			}
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			var ownerTransfer = transaction as OwnerTransferDO;

			if (ownerTransfer != null)
			{
				ownerTransfer.ToCarrierCompanyGuid = newGuid;
			}
		}
	}
}