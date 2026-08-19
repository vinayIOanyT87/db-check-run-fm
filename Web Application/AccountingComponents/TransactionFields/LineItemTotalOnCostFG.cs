namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;

	public class LineItemTotalOnCostFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Construction
		public LineItemTotalOnCostFG()
		{
		}
		#endregion // Construction

		#region Overrides
		public override string FieldID
		{
			get
			{
				return "LineItem TotalOnCost";
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
		#endregion // Overrides

		#region ILineItemField members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (string.IsNullOrWhiteSpace(inLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14]))
			{
				return null;
			}

			double oncost = 0.0;
			try
			{
				oncost = double.Parse(inLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14]);
			}
			catch (Exception)
			{
			}

			return oncost;
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
			// virtual-custom, cannot be set
		}
		#endregion
	}
}
