using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for RouteOriginationDateFG.
	/// </summary>
	internal class RouteOriginationDateFG : DateGenerator, IHeaderField
	{
		public RouteOriginationDateFG()
		{

		}

		public override string FieldID { get { return "RouteOriginationDate"; } }

		public object GetDataValue(TransactionDO transaction)
		{ return transaction.RouteInfo.RouteOriginationDate; }

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
			transaction.RouteInfo.RouteOriginationDate = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
