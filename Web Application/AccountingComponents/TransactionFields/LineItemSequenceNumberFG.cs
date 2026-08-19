namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemSequenceNumberFG.
	/// </summary>
	public class LineItemSequenceNumberFG : TextFieldGenerator, ILineItemField
	{
		public LineItemSequenceNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem LineItemSequenceNumber";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 5.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 5);
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.SequenceId;
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
			var sequenceId = newValue as string;

			if (string.IsNullOrEmpty(sequenceId))
			{
				sequenceId = string.Empty;
			}

			inLineItem.SequenceId = int.Parse(sequenceId);
			OnFieldChanged();
		}
		#endregion
	}
}
