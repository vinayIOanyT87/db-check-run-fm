//*****************************************************************************
// LineItemValueRemainingFG.cs
//
// Original Author: Ivan Orndorff
// Revisions: See source control comments
//
// (C) Copyright 2007 by Varec, Inc.  All rights reserved.
//
//	MODIFICATION HISTORY:
//		Date:		   By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2007-10-05	I. Orndorff			- Initial Revision.
//
//    2009-03-13  Richard Panachida Defect 1938. Added code to check a field being virtual.
//*****************************************************************************

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemValueRemainingFG.
	/// </summary>
	public class LineItemValueRemainingFG : NumericTextFieldGenerator, ILineItemField
	{
		public LineItemValueRemainingFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ValueRemaining";
			}
		}

		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.DEFAULT;
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
			return inLineItem.ValueRemaining;
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
