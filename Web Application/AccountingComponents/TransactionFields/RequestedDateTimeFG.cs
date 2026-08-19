namespace TransactionFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using FMBusinessObjects.DataObjects;

    public class RequestedDateTimeFG : DateTimeGenerator, IHeaderField
	{
		public RequestedDateTimeFG()
		{

		}

		public override string FieldID { get { return "RequestedDateTime"; } }
		public object GetDataValue(TransactionDO transaction)
		{ return transaction.RequestedDateTime; }

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
			transaction.RequestedDateTime = newValue as DateTimeOffset?;
            if (transaction.RequestedDateTime != null)
            {
                this.SetDisplayValue(transaction.RequestedDateTime.Value);
            }
            OnFieldChanged();
		}
	}
}
