namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemCLIN_FG.
	/// </summary>
	public class LineItemCLIN_FG : TextFieldGenerator, ILineItemField
	{
		public LineItemCLIN_FG()
		{

		}

		public override string FieldID
		{
			get
			{
				return "LineItem CLIN";
			}
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

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.CLIN;
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
			inLineItem.CLIN = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
