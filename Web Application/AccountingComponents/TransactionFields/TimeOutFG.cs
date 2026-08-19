using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for TimeOutFG.
	/// </summary>
	public class TimeOutFG : DateTimeGenerator, IHeaderField
	{
		public TimeOutFG()
		{

		}

		public override string FieldID { get { return "TimeOut"; } }
		public object GetDataValue(TransactionDO transaction)
		{ return transaction.TimeOut; }

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
			transaction.TimeOut = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
