namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for Date03FG.
	/// </summary>
	public class Date03FG : DateTimeGenerator, IHeaderField
	{
		public Date03FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Date03";
			}
		}

		#region IHeaderField Members
		public virtual object GetDataValue(TransactionDO transaction)
		{
			return transaction.Date03;
		}

		public virtual string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public virtual void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.Date03 = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
		#endregion
	}
}
