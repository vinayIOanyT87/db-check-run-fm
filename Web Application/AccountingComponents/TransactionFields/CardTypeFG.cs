namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for CardTypeFG.
	/// </summary>
	public class CardTypeFG : TextFieldGenerator, IHeaderField
	{
		public CardTypeFG()
		{
		}

		public override string FieldID { get { return "CardType"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.PaymentInfo.CreditCardType;
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
			transaction.PaymentInfo.CreditCardType = newValue as string;

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
