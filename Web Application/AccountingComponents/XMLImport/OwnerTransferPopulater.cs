/// <summary>
///	File name:	TransactionPopulaterBase.cs
///	Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///					2007.  This file shall not be copied or reproduced in any form 
///					without the express written consent of Varec.
///	Author(s):	Thomas Beckum
///	Version:		7.1.0  Current version
///	
///	Modification History:
///	Date:			By:						Reason:
///	----------	-----------------		-------------------------------------------
///	25-Jan-07	I.Orndorff				1.0.0.1 - Modified "Populate()" removed call
///												to "SetConjoinedTransID()".
///	2007-12-26	Richard Panachida		Added	an updated from 7.0 to check for subtypes in the
///												Populate method.	
///	</summary>
using System;
using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	public class OwnerTransferPopulater : TransactionPopulater
	{
		public OwnerTransferPopulater()
		{
			
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T13_OwnerTransfer;
			}
		}

		protected override void Populate()
		{
			if (this.transaction.TransID.EndsWith("From") == true)
			{
				this.transaction.SubType = "D";
			}
			else if (this.transaction.TransID.EndsWith("To") == true)
			{
				this.transaction.SubType = "C";
			}
		}

		protected override void PopulateLineItem()
		{

		}

	}
}
