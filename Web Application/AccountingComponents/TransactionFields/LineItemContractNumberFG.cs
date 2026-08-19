namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemContractNumberFG.
	/// </summary>
	public class LineItemContractNumberFG : TextFieldGenerator, ILineItemField
	{
		public LineItemContractNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ContractNumber";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 30);
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.ContractNumber;
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
			inLineItem.ContractNumber = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
