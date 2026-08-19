using System;
using FMBusinessObjects.DataObjects;
namespace TransactionFields
{
	/// <summary>
	/// Summary description for Date02FG.
	/// </summary>
	public class Date02FG : DateTimeGenerator, IHeaderField
	{
		public Date02FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Date02";
			}
		}

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Date02;
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
			transaction.Date02 = newValue as DateTimeOffset?;

			OnFieldChanged();
		}

		#endregion
	}
}
