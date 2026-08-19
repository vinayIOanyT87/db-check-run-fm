namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemPitFG.
	/// </summary>
	public class LineItemPitFG : TextFieldGenerator, ILineItemField
	{
		public LineItemPitFG()
		{

		}

		public override string FieldID
		{
			get
			{
				return "LineItem Pit";
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
			return inLineItem.Pit;
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
			inLineItem.Pit = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
