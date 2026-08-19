namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LineItemBaseCostFG : NumericTextFieldGenerator, ILineItemField
	{
		#region Construction
		public LineItemBaseCostFG()
		{
			virtualField = true;
		}
		#endregion // Construction

		#region Overrides
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

		public override bool Editable
		{
			get
			{
				return false;
			}
			set
			{
				base.Editable = value;
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem BaseCost";
			}
		}
		#endregion // Overrides

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.ProductPrice == null)
			{
				return null;
			}

			double basePrice;

			if (this.trans.TransTypeID != TransactionTypes.T8_Receipt)
			{
				basePrice = inLineItem.Tax4 == null ? 0.0 : inLineItem.Tax4.Value;
			}
			else
			{
				basePrice = inLineItem.Number06 == null ? 0.0 : inLineItem.Number06.Value;
			}

			return basePrice;
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
			// will never be set
		}
		#endregion
	}
}
