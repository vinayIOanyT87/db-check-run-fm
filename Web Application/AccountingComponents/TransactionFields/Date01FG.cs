using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for Date01FG.
	/// </summary>
	public class Date01FG : DateTimeGenerator, IHeaderField
	{
		public Date01FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Date01";
			}
		}

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Date01;
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
			transaction.Date01 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}

		#endregion
	}
}
