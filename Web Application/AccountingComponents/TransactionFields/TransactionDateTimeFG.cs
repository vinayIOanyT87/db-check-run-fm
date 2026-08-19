using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for TransactionDateTimeFG.
	/// </summary>
	public class TransactionDateTimeFG : DateTimeGenerator, IHeaderField
	{
		public override string FieldID { get { return "TransDateTime"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TransactionDateTime;
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
			transaction.TransactionDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
