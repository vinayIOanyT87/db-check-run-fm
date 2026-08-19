using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	class DispatchedDateTimeFG : DateTimeGenerator, IHeaderField
	{
		public DispatchedDateTimeFG()
		{
		}

		public override string FieldID { get { return "DispatchedDateTime"; } }
		public object GetDataValue(TransactionDO transaction)
		{ return transaction.DispatchedDateTime; }

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
			transaction.DispatchedDateTime = newValue as DateTimeOffset?;

			OnFieldChanged();
		}
	}
}
