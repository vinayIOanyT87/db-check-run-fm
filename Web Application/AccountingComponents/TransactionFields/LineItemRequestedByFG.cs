namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemRequestedByFG.
	/// </summary>
	public class LineItemRequestedByFG : TextFieldGenerator, ILineItemField
	{
		public LineItemRequestedByFG()
		{

		}

		public override string FieldID
		{
			get
			{
				return "LineItem RequestedBy";
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
			return inLineItem.RequestedBy;
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
			inLineItem.RequestedBy = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
