using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for ETA_FG.
	/// </summary>
	public class ETA_FG : DateTimeGenerator, IHeaderField
	{
		public ETA_FG()
		{

		}

		public override string FieldID
		{ get { return "ETA"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteSchedule.ETA;
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
			transaction.RouteSchedule.ETA = newValue as DateTimeOffset?;

			OnFieldChanged();
		}
	}
}
