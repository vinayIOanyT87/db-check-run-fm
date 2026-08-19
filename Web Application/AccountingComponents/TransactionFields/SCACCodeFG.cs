namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for SCACCodeFG.
	/// </summary>
	public class SCACCodeFG : TextFieldGenerator, IHeaderField
	{
		public SCACCodeFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "SCACCode";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.SCACCode;
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
			transaction.SCACCode = newValue as string;
			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 4.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 4);
			}
		}
	}
}
