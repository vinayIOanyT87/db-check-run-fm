 #pragma warning disable 1587
/// <summary>
/// File name:	LoadAuthorizationRecordDTN.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2011.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Warren Gray
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		
/// </summary>
/// 
#pragma warning restore 1587

namespace FMBusinessObjects.PIDXTransactions
{
    // ReSharper disable once InconsistentNaming
    public class LoadAuthorizationRecordDTN : LoadAuthorizationRecord
	{
		#region Properties
		public override string ConsigneeNumber
		{
			get { return this.consigneeNumber.PadLeft(14,' '); }
			set { this.consigneeNumber = value.Substring(0,(value.Length < 14) ? value.Length : 14); }
		}
		#endregion
	}
}
