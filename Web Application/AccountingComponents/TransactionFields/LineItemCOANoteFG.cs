namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemCOANoteFG.
	/// </summary>
	public class LineItemCOANoteFG : TextFieldGenerator, ILineItemField
	{
		public LineItemCOANoteFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem COANote";
			}
		}

		public override bool Editable
		{
			get
			{
				return true;
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
			return inLineItem.COANote;
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
			inLineItem.COANote = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
