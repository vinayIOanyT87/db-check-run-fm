namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ReversalTypeFG.
	/// </summary>
	public class ReversalTypeFG : TextFieldGenerator, IHeaderField
	{
		public ReversalTypeFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "ReversalType";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.ReversalType;
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
			OnFieldChanged();
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
		/// default length of 2.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 2);
			}
		}
	}
}
