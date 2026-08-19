namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for CardNameFG.
	/// </summary>
	public class CardNameFG : TextFieldGenerator, IHeaderField
	{
		public CardNameFG()
		{
		}

		public override string FieldID { get { return "CardName"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.PaymentInfo.CreditCardName;
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
			transaction.PaymentInfo.CreditCardName = newValue as string;

			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, 30); }
		}
	}
}
