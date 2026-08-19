 #pragma warning disable 1587
/// <summary>
/// File name:	CreditAuthorizationRecordDTN.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		19-Mar-08	I.Orndorff		1.0.0 - Initial Revision.
///		
/// </summary>
/// 
#pragma warning restore 1587

namespace FMBusinessObjects.PIDXTransactions
{
    // ReSharper disable once InconsistentNaming
    public class CreditAuthorizationRecordDTN : CreditAuthorizationRecord
	{
		#region Properties
		public override string ConsigneeNumber
		{
			get { return base.consigneeNumber.PadLeft(14,' '); }
			set { base.consigneeNumber = value.Substring(0,(value.Length < 14) ? value.Length : 14); }
		}
		#endregion
	}
}
