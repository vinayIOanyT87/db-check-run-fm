using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for FST_FG.
	/// </summary>
	public class FST_FG : DateTimeGenerator, IHeaderField
	{
		public FST_FG()
		{

		}

		public override string FieldID
		{ get { return "FST"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteSchedule.FST;
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
			transaction.RouteSchedule.FST = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
