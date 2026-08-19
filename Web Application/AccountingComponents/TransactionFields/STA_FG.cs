using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for STA_FG.
	/// </summary>
	public class STA_FG : DateTimeGenerator, IHeaderField
	{
		public STA_FG()
		{

		}

		public override string FieldID
		{ get { return "STA"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteSchedule.STA;
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
			transaction.RouteSchedule.STA = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
