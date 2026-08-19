namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ShipmentNumberFG.
	/// </summary>
	public class ShipmentNumberFG : TextFieldGenerator, IHeaderField
	{
		public ShipmentNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "ShipmentNumber";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.ShipmentNumber;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.ShipmentNumber = newValue as string;
			OnFieldChanged();
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
	}
}
