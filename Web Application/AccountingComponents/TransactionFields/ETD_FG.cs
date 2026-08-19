using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for ETD_FG.
	/// </summary>
	public class ETD_FG : DateTimeGenerator, IHeaderField
	{
		public ETD_FG()
		{

		}

		public override string FieldID
		{ get { return "ETD"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteSchedule.ETD;
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
			transaction.RouteSchedule.ETD = newValue as DateTimeOffset?;

			OnFieldChanged();
		}
	}
}
