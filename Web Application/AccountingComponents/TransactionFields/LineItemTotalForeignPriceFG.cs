namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LineItemTotalForeignPriceFG : TextFieldGenerator, ILineItemField
	{
		public LineItemTotalForeignPriceFG()
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

		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength("TotalForeignPrice", 30);
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem TotalForeignPrice";
			}
		}

		#region ILineItemField members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_03];
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
			// do nothing, this is a virtual field
		}
		#endregion
	}
}
