namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for PONFG.
	/// </summary>
	public class PONFG : TextFieldGenerator, IHeaderField
	{
		public PONFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "PONumber";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.PONumber;
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
			transaction.PONumber = newValue as string;
			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 14.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 14);
			}
		}
	}
}
