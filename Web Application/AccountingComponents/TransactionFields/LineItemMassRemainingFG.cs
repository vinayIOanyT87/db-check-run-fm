//******************************************************************************
//	FILE NAME:		LineItemMassRemainingFG.cs
//	PURPOSE:		This class inherits from NumericTextFieldGenerator and sets
//					the Mass value.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Sijuan Jiang
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		
//*******************************************************************************       

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;

	/// <summary>
	/// Summary description for LineItemMassRemainingFG.
	/// </summary>
	public class LineItemMassQuantityRemainingFG : LineItemMassFG, ILineItemField
	{
		public LineItemMassQuantityRemainingFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem MassQuantityRemaining";
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
				return false;
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return Math.Round(inLineItem.MassQuantityRemaining, inLineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
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
			// Calculated field value - not setable
			OnFieldChanged();
		}
	}
}
