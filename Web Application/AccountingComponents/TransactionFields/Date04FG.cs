using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for Date04FG.
	/// </summary>
	public class Date04FG : DateTimeGenerator, IHeaderField
	{
		public Date04FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Date04";
			}
		}

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Date04;
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
			transaction.Date04 = newValue as DateTimeOffset?;

			OnFieldChanged();
		}

		#endregion
	}
}
