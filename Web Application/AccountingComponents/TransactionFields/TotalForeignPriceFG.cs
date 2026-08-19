namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;

	public class TotalForeignPriceFG : NumericTextFieldGenerator, IHeaderField
	{
		public TotalForeignPriceFG()
		{
			virtualField = true;
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		public override string FieldID
		{
			get
			{
				return "TotalForeignPrice";
			}
		}

		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(this.FieldID, 30);
			}
		}

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

		#region IHeaderField methods
		public object GetDataValue(TransactionDO transaction)
		{
			double total = 0.0;

			foreach (LineItemDO li in transaction.LineItems)
			{
				try
				{
					total += string.IsNullOrWhiteSpace(li.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_03]) ?
						0.0 : double.Parse(li.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_03]);
				}
				catch (Exception)
				{
					// trap oob and parse errors for transactions not meant to be in user data 3
				}
			}

			return total;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			// virtual field, no value setting capability
		}
		#endregion // IHeaderField methods
	}
}
