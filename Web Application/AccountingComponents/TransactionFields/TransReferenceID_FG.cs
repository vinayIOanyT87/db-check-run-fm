namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TransReferenceID_FG.
	/// </summary>
	public class TransReferenceID_FG : TextFieldGenerator, IHeaderField
	{
		public TransReferenceID_FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "TransReferenceID";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TransRefID;
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

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 64.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 64);
			}
		}
	}
}
