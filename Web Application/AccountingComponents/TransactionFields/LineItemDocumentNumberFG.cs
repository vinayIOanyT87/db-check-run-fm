namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemDocumentNumberFG.
	/// </summary>
	public class LineItemDocumentNumberFG : TextFieldGenerator, ILineItemField
	{
		public LineItemDocumentNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem DocumentNumber";
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.DocumentNumber;
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
			inLineItem.DocumentNumber = newValue as string;
			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 10.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 10);
			}
		}
	}
}
