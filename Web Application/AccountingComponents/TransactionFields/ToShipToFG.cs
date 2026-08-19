//*****************************************************************************************************************
//  FILE NAME:		ToShipToFG.cs
//	PURPOSE:		This class inherits from the ShipToFG class. It is used during
//					consumer transfers.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Richard Panachida
//	VERSION:	1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2006-11-02	Richard Panachida	Corrected the defect for the missing ToShipTo (CSI 3575).
//*****************************************************************************************************************
namespace TransactionFields
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ToShipToFG.
	/// </summary>
	public class ToShipToFG : ShipToFG
	{
		public ToShipToFG()
		{
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x =>x.IsADFKey()))
			{
				companySubRole = ADF_SUBROLE;
			}
		}

		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{ 
			get { return "ToShipToID"; } 
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, FIELD_LENGTH); }
		}

		protected override CompanyCollectionClass GetEntries()
		{
			CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(transContext.security, COMPANY_ROLE.CUSTOMER_SHIPTO,false,false)
																);

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				var consumerTransfer = trans as ConsumerTransferDO;

				if (consumerTransfer != null && (consumerTransfer.BillToCompanyGuid == Guid.Empty || 
												this.trans.ShipperCompanyGuid == Guid.Empty || 
												this.trans.OwnerCompanyGuid == Guid.Empty || 
												this.trans.ManagerCompanyGuid == Guid.Empty))
				{
					companyCollection.Clear();
				}
				else
				{
					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, 
																									trans.ManagerCompanyGuid, 
																									trans.OwnerCompanyGuid, 
																									COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						companyCollection.Clear();
					}
					else
					{
						companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, 
																									companyMapGuid, 
																									trans.ShipperCompanyGuid, 
																									COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);

						if (companyMapGuid == Guid.Empty)
						{
							companyCollection.Clear();
						}
						else
						{
							companyMapGuid =
								FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
									x =>
									consumerTransfer != null ? x.GetIdentityGuidByGuidsAndType(
										this.transContext.security, companyMapGuid, consumerTransfer.BillToCompanyGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP) : new Guid());

							if (companyMapGuid == Guid.Empty)
							{
								companyCollection.Clear();
							}
							else
							{
								CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
									x =>
										x.EnumerateByAssignedToGuidAndType(this.transContext.security, companyMapGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP));

								var limitedCompanyCollection = new CompanyCollectionClass();

								foreach (CompanyClass company in companyCollection)
								{
									if (companyMapCollection.Find(company.MasterRecordGuid) == null)
									{
										continue;
									}

									limitedCompanyCollection.Add(company);
								}

								companyCollection = limitedCompanyCollection;
							}
						}
					}
				}
			}

			return companyCollection;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;

			if (consumerTransfer != null)
			{
				return consumerTransfer.ToShipToID;
			}

			return string.Empty;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;

			if (consumerTransfer != null)
			{
				consumerTransfer.ToShipToCode = newCode;
			}
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;

			if (consumerTransfer != null)
			{
				consumerTransfer.ToShipToID = newID;
			}
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;
			
			if (consumerTransfer != null)
			{
				consumerTransfer.ToShipToCompanyGuid = newGuid;
			}
		}
	}
}
