namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemTankLevelUnitsFG.
	/// 05-22-2008 V. Thompson
	/// Line Item field added for ADF
	/// </summary>
	public class LineItemInvoiceNumberFG : TextFieldGenerator, ILineItemField
	{
		public LineItemInvoiceNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem InvoiceNumber";
			}
		}

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

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.InvoiceNumber;
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
			inLineItem.InvoiceNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
