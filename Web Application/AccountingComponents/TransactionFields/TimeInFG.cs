using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for TimeInFG.
	/// </summary>
	public class TimeInFG : DateTimeGenerator, IHeaderField
	{
		public TimeInFG()
		{

		}

		public override string FieldID { get { return "TimeIn"; } }
		public object GetDataValue(TransactionDO transaction)
		{ return transaction.TimeIn; }

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.TimeIn = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
