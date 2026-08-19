//*****************************************************************************************************************
//  FILE NAME:		ToBillToFG.cs
//	PURPOSE:		This class inherits from the BillToFG class. It is used during
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
//		2006-11-02	Richard Panachida	Corrected the defect for the missing ToBillTo (CSI 3575).
//		2006-11-29	Richard Panachida	Modification to use the company text box button
//										combo field (CSI 3644).
//*****************************************************************************************************************
namespace TransactionFields
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ToBillToFG.
	/// </summary>
	public class ToBillToFG : BillToFG
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor for the ToBillToFG class.
		/// </summary>
		public ToBillToFG()
		{
			this.companyRole = BILLTO_ROLE;

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x =>x.IsADFKey()))
			{
				companySubRole = ADF_SUBROLE;
			}
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the AutoPostBack
		/// </summary>
		protected override bool AutoPostBack
		{
			get
			{
				return (transContext.aliasClass.LimitSelectionsBasedOnHierarchy) ? true : false;
			}
		}

		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{ 
			get { return "ToBillToID"; } 
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, FIELD_LENGTH); }
		}
		#endregion

		#region Override Methods
		public override object GetDataValue(TransactionDO transaction)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;

			if (consumerTransfer != null)
			{
				return consumerTransfer.ToBillToID;
			}

			return string.Empty;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			this.SetValue(newValue);

			var consumerTransfer = transaction as ConsumerTransferDO;

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find("ToShipToID") == null)
				{
					return;
				}

				var toShipToFG = fieldGenerator.GetFieldGenerator("ToShipToID") as CompanyTextButtonGenerator;

				if (consumerTransfer != null && (consumerTransfer.ToBillToCompanyGuid == Guid.Empty
				                                 || transaction.ShipperCompanyGuid == Guid.Empty
				                                 || transaction.OwnerCompanyGuid == Guid.Empty
				                                 || transaction.ManagerCompanyGuid == Guid.Empty))
				{
					if (toShipToFG != null)
					{
						toShipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	x =>
																	x.GetIdentityGuidByGuidsAndType(transContext.security, 
																									transaction.ManagerCompanyGuid, 
																									transaction.OwnerCompanyGuid, 
																									COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

				if (companyMapGuid == Guid.Empty)
				{
					if (toShipToFG != null)
					{
						toShipToFG.SetDataValue(transaction, string.Empty);
					}
					return;
				}

				companyMapGuid =
					FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
						x =>
						x.GetIdentityGuidByGuidsAndType(
							transContext.security, companyMapGuid, transaction.ShipperCompanyGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP));

				if (companyMapGuid == Guid.Empty)
				{
					if (toShipToFG != null)
					{
						toShipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				companyMapGuid =
					FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
						x =>
						consumerTransfer != null ? x.GetIdentityGuidByGuidsAndType(
							this.transContext.security, companyMapGuid, consumerTransfer.ToBillToCompanyGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP) : new Guid());

				if (companyMapGuid == Guid.Empty)
				{
					if (toShipToFG != null)
					{
						toShipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				CompanyMapCollectionClass companyMapCollection =
					FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x => x.EnumerateByAssignedToGuidAndType(transContext.security, companyMapGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP));

				if (toShipToFG != null)
				{
					if (companyMapCollection.Count == 0)
					{
						toShipToFG.SetDataValue(transaction, string.Empty);
					}
					else if (companyMapCollection.Count == 1)
					{
						if (transaction.ShipToCompanyGuid == Guid.Empty
						    || transaction.ShipToCompanyGuid != companyMapCollection[0].AssignedGuid)
						{
							toShipToFG.SetDataValue(transaction, companyMapCollection[0].AssignedID);
						}
					}
					else if (transaction.ShipToCompanyGuid == Guid.Empty
					         || companyMapCollection.Find(transaction.ShipToCompanyGuid) == null)
					{
						toShipToFG.SetDataValue(transaction, string.Empty);
					}
				}
			}
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;
			
			if (consumerTransfer != null)
			{
				consumerTransfer.ToBillToCode = newCode;
			}
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;
			
			if (consumerTransfer != null)
			{
				consumerTransfer.ToBillToID = newID;
			}
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			var consumerTransfer = transaction as ConsumerTransferDO;
			
			if (consumerTransfer != null)
			{
				consumerTransfer.ToBillToCompanyGuid = newGuid;
			}
		}
		#endregion
	}
}
