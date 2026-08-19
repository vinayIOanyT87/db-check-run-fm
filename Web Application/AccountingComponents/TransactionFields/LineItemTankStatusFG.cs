//******************************************************************************
//	FILE NAME:		TransactionProcessor.cs
//	PURPOSE:		This class handles the retrieving of the transaction data.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Thomas Beckum
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:				Reason:
//		---------	-----------------	-------------------------------------------
//		2008-08-22	W.Gray				- 7.4.6.0 Changed tankStatus from char to string (CSI 6072)
//*******************************************************************************       

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemTankStatusFG.
	/// </summary>
	public class LineItemTankStatusFG : TextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemTankStatusFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem TankStatus";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 30);
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (string.IsNullOrEmpty(inLineItem.TankStatus))
			{
				return null;
			}

			return inLineItem.TankStatus;
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
			inLineItem.TankStatus = string.Format("{0}", newValue);
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.TankStatus;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}
			
			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
		    inSublineItem.TankStatus = string.Format("{0}", newValue);
			OnFieldChanged();
		}
		#endregion
	}
}
