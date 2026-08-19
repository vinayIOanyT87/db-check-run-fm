namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ExpirationDateFG.
	/// </summary>
	internal class ExpirationDateFG : DateGenerator, IHeaderField
	{
		public ExpirationDateFG()
		{
		}

		public override string FieldID
		{
			get { return "ExpirationDate"; }
		}

		public override bool Required
		{
			get { return false; }
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.ExpirationDate;
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
			transaction.ExpirationDate = newValue as DateTimeOffset?;
			base.OnFieldChanged();
		}
	}
}
