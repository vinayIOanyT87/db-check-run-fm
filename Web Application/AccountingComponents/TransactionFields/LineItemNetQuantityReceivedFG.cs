/// <summary>
/// File name:	LineItemNetQuantityReceivedFG.cs
/// Purpose:	
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2007-09-18		I.Orndorff				7.3.0.0 - Added new transaction type (T18_SupplyOrder). 
/// </summary>
namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;

	/// <summary>
	/// Summary description for LineItemNetQuantityReceivedFG.
	/// </summary>
	internal class LineItemNetQuantityReceivedFG : QuantityReceivedFG, ILineItemField
	{
		public LineItemNetQuantityReceivedFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem NetQuantityReceived";
			}
		}

		public override bool Required
		{
			get
			{
				return false;
			}
		}

		public override bool Editable
		{
			get
			{
				// Only editable if this is a Order type transaction
				if (trans != null &&
					trans.TransTypeID != TransactionTypes.T17_Order && 
					trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
				{
					return false;
				}

				var transactionDO = this.trans;

				if (transactionDO != null)
				{
					LineItemDO localLineItem = transactionDO.LineItems[0];
					return localLineItem.TransactionLineItemGuid != Guid.Empty;
				}

				return false;
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return Math.Round(inLineItem.NetQuantityReceived, inLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			OnFieldChanged();
		}
	}
}
