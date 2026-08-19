/// <summary>
/// File name:	ToOwnerFG.cs
/// Purpose:	The purpose of this class is to define the Owner field.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2006-11-29		Richard Panachida		Modification to use the company text box button
///												combo field (CSI 3644).
/// </summary>
using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	public class ToOwnerFG : OwnerFG
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor for the OwnerFG class.
		/// </summary>
		public ToOwnerFG()
		{
			base.companyRole = CompanyTextButtonGenerator.OWNER_ROLE;
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID 
		{ 
			get { return "ToOwnerID"; } 
		}

		/// <summary>
		/// This property return true meaning that the field is required.
		/// </summary>
		public override bool Required 
		{ 
			get { return true; } 
		}
		#endregion

		#region Override Methods
		public override object GetDataValue(TransactionDO transaction)
		{
			OwnerTransferDO ownerTransfer = transaction as OwnerTransferDO;
			return ownerTransfer.ToOwnerID;
		}

		protected override void SetCompanyCode(TransactionDO trans, string newCode)
		{
			OwnerTransferDO ownerTransfer = trans as OwnerTransferDO;
			ownerTransfer.ToOwnerCode = newCode;
		}

		protected override void SetCompanyID(TransactionDO trans, string newID)
		{
			OwnerTransferDO ownerTransfer = trans as OwnerTransferDO;
			ownerTransfer.ToOwnerID = newID;
		}

		protected override void SetCompanyGuid(TransactionDO trans, Guid newGuid)
		{
			OwnerTransferDO ownerTransfer = trans as OwnerTransferDO;
			ownerTransfer.ToOwnerCompanyGuid = newGuid;
		}


		#endregion
	}
}
