namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TransactionTypeFG.
	/// </summary>
	public class TransactionTypeFG : TextFieldGenerator, IHeaderField
	{
		public TransactionTypeFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LookupTransTypeIndex";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TransTypeID.ToString();
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
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 25.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 25);
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}
	}
}
