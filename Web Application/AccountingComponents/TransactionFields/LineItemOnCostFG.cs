namespace TransactionFields
{
	using System;
	using System.Globalization;

	using FMBusinessObjects.DataObjects;

	public class LineItemOnCostFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the line item on-cost field generator.
		/// </summary>
		public LineItemOnCostFG()
		{
		}
		#endregion

		#region Override properties
		/// <summary>
		/// This property will return the ID of the field.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem OnCost";
			}
		}

		/// <summary>
		/// This property will return the type of the field (double).
		/// </summary>
		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		/// <summary>
		/// This property will return the unit type which is set to default.
		/// </summary>
		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.DEFAULT;
			}
		}
		#endregion

		#region ILineItemField Members
		/// <summary>
		/// This method will return either null if there is no value or the 
		/// actual value as a double?.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(LineItemDO inLineItem)
		{
			double result;

			string onCostValue = inLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14];

			if (string.IsNullOrEmpty(onCostValue))
			{
				result = 0f;
			}
			else
			{
				try
				{
					result = double.Parse(onCostValue);
				}
				catch (Exception)
				{
					result = 0f;
				}
			}

			return result;
		}

		/// <summary>
		/// This method will return the actual value as a string.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		/// <summary>
		/// This method will set the new value in the object.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14] = "0.00";
			}
			else
			{
				inLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14] = ((double) newValue).ToString(CultureInfo.InvariantCulture);
			}
			OnFieldChanged();
		}
      #endregion
   }
}
