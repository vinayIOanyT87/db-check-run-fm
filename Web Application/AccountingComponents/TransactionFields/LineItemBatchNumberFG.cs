namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class LineItemBatchNumberFG : TextFieldGenerator, ILineItemField, ISublineItemField
	{
		/// <summary>
		/// This is the default constructor for the Line Item Batch Number class.
		/// </summary>
		public LineItemBatchNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem BatchNumber";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 16.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 16);
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.BatchNumber;
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
			inLineItem.BatchNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.BatchNumber;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}

			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.BatchNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
