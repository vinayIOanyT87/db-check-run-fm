using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for SFT_FG.
	/// </summary>
	public class SFT_FG : DateTimeGenerator, IHeaderField
	{
		public SFT_FG()
		{

		}

		public override string FieldID
		{ get { return "SFT"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteSchedule.SFT;
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
			transaction.RouteSchedule.SFT = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
