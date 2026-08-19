using System;
using System.Collections.Generic;
using System.Text;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	public class ADFTransactionDateTimeFG : DateTimeGenerator, IHeaderField
	{
		public ADFTransactionDateTimeFG()
			: base()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "ADFTransactionDateTime";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.TransactionDateTime;
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
			transaction.TransactionDateTime = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
