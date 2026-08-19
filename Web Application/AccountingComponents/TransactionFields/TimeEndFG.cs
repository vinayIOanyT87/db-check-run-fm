using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for TimeEndFG.
	/// </summary>
	public class TimeEndFG : DateTimeGenerator, IHeaderField
	{
		public TimeEndFG()
		{

		}

		public override string FieldID { get { return "TimeEnd"; } }
		public object GetDataValue(TransactionDO transaction)
		{ return transaction.TimeEnd; }

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
			transaction.TimeEnd = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
