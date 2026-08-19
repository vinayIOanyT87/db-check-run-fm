using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for STD_FG.
	/// </summary>
	public class STD_FG : DateTimeGenerator, IHeaderField
	{
		public STD_FG()
		{

		}

		public override string FieldID
		{ get { return "STD"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteSchedule.STD;
		}

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
			transaction.RouteSchedule.STD = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
