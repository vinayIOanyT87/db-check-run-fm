namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;

	/// <summary>
	/// Summary description for EffectiveDateFG.
	/// </summary>
	internal class EffectiveDateFG : DateGenerator, IHeaderField
	{
		public EffectiveDateFG()
		{
		}

		public override string FieldID
		{
			get { return "EffectiveDate"; }
		}


		public override bool Required
		{
			get { return false; }
		}


		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.EffectiveDate;
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
			transaction.EffectiveDate = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
