///*****************************************************************************
///
/// LineItemLocationFG
/// 
/// Original Author: Van Thompson
/// Revisions: See source control comments
/// 
/// (C) Copyright 2008 by Varec, Inc.  All rights reserved.
/// 
/// Revision History
/// Date:		   By:				Reason:
/// 2009-02-26    G.Kendall      WI#1431 - Increased IATAID to 50 characters.  Also
///                              set to read-only if no entries for the drop down.
/// 
/// *****************************************************************************

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemLocationFG.
	/// </summary>
	public class LineItemDeliveryLocationFG : RouteStationGenerator, ILineItemField
	{
		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 50.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 50);
			}
		}

		public override bool Editable
		{
			get
			{
				return base.Editable && HasEntries;
			}
			set
			{
				base.Editable = value;
			}
		}

		public LineItemDeliveryLocationFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem DeliveryLocation";
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.DeliveryLocation;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public virtual void SetDataValue(LineItemDO inLineItem, object newValue)
		{
            lineItem.DeliveryLocation = string.Format("{0}", newValue);
            OnFieldChanged();
		}
		#endregion
	}
}
