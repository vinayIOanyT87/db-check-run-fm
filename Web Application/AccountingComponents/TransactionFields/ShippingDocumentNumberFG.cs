namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ShippingDocumentNumber.
	/// </summary>
	public class ShippingDocumentNumberFG : TextFieldGenerator, IHeaderField
	{
		public ShippingDocumentNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "ShippingDocumentNumber";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.ShippingDocumentNumber;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public virtual void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.ShippingDocumentNumber = newValue as string;
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
