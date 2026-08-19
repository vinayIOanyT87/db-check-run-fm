namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for TicketMode.
	/// </summary>
	public class TicketMode : TextFieldGenerator, IHeaderField
	{
		public TicketMode()
		{
		}

		public override string FieldID
		{
			get
			{
				return "TicketMode";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TicketMode.ToString();
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
		/// default length of 7.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 7);
			}
		}
	}
}
