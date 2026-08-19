/// <summary>
/// File name:	PhysicalInventoryPopulater.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2007.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec.
///	Author(s):	
///	Version:	7.1.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		25-Jan-07	I.Orndorff		1.0.0.1 - Removed commented out "SetOwner()" 
///											  method.
///		
///	</summary>
///

using System;
using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for PhysicalInventoryPopulater.
	/// </summary>
	public class PhysicalInventoryPopulater : TransactionPopulater
	{
		public PhysicalInventoryPopulater()
		{
			
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T14_PhysicalInventory;
			}
		}

		protected override void Populate()
		{

		}

		protected override void PopulateLineItem()
		{
			SetLineItemLineFill();
			SetLineItemBottomVolume();
			SetLineItemNetCapacity();
			SetLineItemTankStatus();
		}
	}
}
